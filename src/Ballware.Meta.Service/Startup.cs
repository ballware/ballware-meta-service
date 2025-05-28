using Ballware.Meta.Api;
using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Authorization;
using Ballware.Meta.Authorization.Jint;
using Ballware.Meta.Caching;
using Ballware.Meta.Data.Ef;
using Ballware.Meta.Data.Ef.Configuration;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Jobs;
using Ballware.Meta.Service.Adapter;
using Ballware.Meta.Service.Configuration;
using Ballware.Meta.Service.Extensions;
using Ballware.Schema.Client;
using Ballware.Storage.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Quartz;
using Serilog;

namespace Ballware.Meta.Service;


public class Startup(IWebHostEnvironment environment, ConfigurationManager configuration, IServiceCollection services)
{
    private IWebHostEnvironment Environment { get; } = environment;
    private ConfigurationManager Configuration { get; } = configuration;
    private IServiceCollection Services { get; } = services;

    public void InitializeServices()
    {
        Services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null; // = PascalCase
        });
        
        Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = null; // = PascalCase
        });
        
        CorsOptions? corsOptions = Configuration.GetSection("Cors").Get<CorsOptions>();
        AuthorizationOptions? authorizationOptions =
            Configuration.GetSection("Authorization").Get<AuthorizationOptions>();
        StorageOptions? storageOptions = Configuration.GetSection("Storage").Get<StorageOptions>();
        CacheOptions? cacheOptions = Configuration.GetSection("Cache").Get<CacheOptions>();
        SwaggerOptions? swaggerOptions = Configuration.GetSection("Swagger").Get<SwaggerOptions>();
        
        ServiceClientOptions? storageClientOptions = Configuration.GetSection("StorageClient").Get<ServiceClientOptions>();
        ServiceClientOptions? schemaClientOptions = Configuration.GetSection("SchemaClient").Get<ServiceClientOptions>();
        
        var metaConnectionString = Configuration.GetConnectionString("MetaConnection");

        Services.AddOptionsWithValidateOnStart<AuthorizationOptions>()
            .Bind(Configuration.GetSection("Authorization"))
            .ValidateDataAnnotations();

        Services.AddOptionsWithValidateOnStart<StorageOptions>()
            .Bind(Configuration.GetSection("Storage"))
            .ValidateDataAnnotations();
        
        Services.AddOptionsWithValidateOnStart<Ballware.Meta.Caching.Configuration.CacheOptions>()
            .Bind(Configuration.GetSection("Cache"))
            .ValidateDataAnnotations();
        
        Services.AddOptionsWithValidateOnStart<CacheOptions>()
            .Bind(Configuration.GetSection("Cache"))
            .ValidateDataAnnotations();

        Services.AddOptionsWithValidateOnStart<SwaggerOptions>()
            .Bind(Configuration.GetSection("Swagger"))
            .ValidateDataAnnotations();
        
        Services.AddOptionsWithValidateOnStart<ServiceClientOptions>()
            .Bind(Configuration.GetSection("StorageClient"))
            .ValidateDataAnnotations();
        
        Services.AddOptionsWithValidateOnStart<ServiceClientOptions>()
            .Bind(Configuration.GetSection("SchemaClient"))
            .ValidateDataAnnotations();

        if (authorizationOptions == null || storageOptions == null || string.IsNullOrEmpty(metaConnectionString))
        {
            throw new ConfigurationException("Required configuration for authorization and storage is missing");
        }

        if (cacheOptions == null)
        {
            cacheOptions = new CacheOptions();
        }
        
        if (storageClientOptions == null)
        {
            throw new ConfigurationException("Required configuration for storageClient is missing");
        }
        
        if (schemaClientOptions == null)
        {
            throw new ConfigurationException("Required configuration for schemaClient is missing");
        }

        Services.Configure<QuartzOptions>(Configuration.GetSection("Quartz"));
        
        Services.AddLogging();
        Services.AddMemoryCache();
        
        if (!string.IsNullOrEmpty(cacheOptions.RedisConfiguration))
        {
            Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheOptions.RedisConfiguration;
                options.InstanceName = cacheOptions.RedisInstanceName;
            });
        }
        else
        {
            Services.AddDistributedMemoryCache();
        }

        Services.AddBallwareDistributedCaching();
        
        Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.Authority = authorizationOptions.Authority;
            options.Audience = authorizationOptions.Audience;
            options.RequireHttpsMetadata = authorizationOptions.RequireHttpsMetadata;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            {
                ValidIssuer = authorizationOptions.Authority
            };
        });

        Services.AddAuthorizationBuilder()
            .AddPolicy("metaApi", policy => policy.RequireAssertion(context =>
                context.User
                    .Claims
                    .Where(c => "scope" == c.Type)
                    .SelectMany(c => c.Value.Split(' '))
                    .Any(s => s.Equals(authorizationOptions.RequiredMetaScope, StringComparison.Ordinal)))
            )
            .AddPolicy("serviceApi", policy => policy.RequireAssertion(context =>
                context.User
                    .Claims
                    .Where(c => "scope" == c.Type)
                    .SelectMany(c => c.Value.Split(' '))
                    .Any(s => s.Equals(authorizationOptions.RequiredServiceScope, StringComparison.Ordinal))));

        if (corsOptions != null)
        {
            Services.AddCors(options =>
            {
                options.AddDefaultPolicy(c =>
                {
                    c.WithOrigins(corsOptions.AllowedOrigins)
                        .WithMethods(corsOptions.AllowedMethods)
                        .WithHeaders(corsOptions.AllowedHeaders);
                });
            });
        }

        Services.AddHttpContextAccessor();

        Services.AddMvcCore()
            .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null)
            .AddApiExplorer();

        Services.AddControllers()
            .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);
        
        
        Services.AddClientCredentialsTokenManagement()
            .AddClient("storage", client =>
            {
                client.TokenEndpoint = storageClientOptions.TokenEndpoint;

                client.ClientId = storageClientOptions.ClientId;
                client.ClientSecret = storageClientOptions.ClientSecret;

                client.Scope = storageClientOptions.Scopes;
            });
        
        Services.AddClientCredentialsTokenManagement()
            .AddClient("schema", client =>
            {
                client.TokenEndpoint = schemaClientOptions.TokenEndpoint;

                client.ClientId = schemaClientOptions.ClientId;
                client.ClientSecret = schemaClientOptions.ClientSecret;

                client.Scope = schemaClientOptions.Scopes;
            });
        
        Services.AddHttpClient<BallwareStorageClient>(client =>
            {
                client.BaseAddress = new Uri(storageClientOptions.ServiceUrl);
            })
            .AddClientCredentialsTokenHandler("storage");
        
        Services.AddHttpClient<BallwareSchemaClient>(client =>
            {
                client.BaseAddress = new Uri(schemaClientOptions.ServiceUrl);
            })
#if DEBUG            
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
#endif            
            .AddClientCredentialsTokenHandler("schema");
        
        Services.AddAutoMapper(config =>
        {
            config.AddBallwareStorageMappings();
            config.AddBallwareMetaApiMappings();
        });
        
        Services.AddScoped<IMetaFileStorageAdapter, StorageServiceFileStorageAdapter>();
        Services.AddScoped<IJobsFileStorageAdapter, StorageServiceFileStorageAdapter>();
        Services.AddScoped<IRepositoryHook<Ballware.Meta.Data.Public.Tenant, Ballware.Meta.Data.Persistables.Tenant>, GenericSchemaTenantRepositoryHook>();
        Services.AddScoped<ITenantableRepositoryHook<Ballware.Meta.Data.Public.EntityMetadata, Ballware.Meta.Data.Persistables.EntityMetadata>, GenericSchemaEntityRepositoryHook>();
        
        Services.AddBallwareMetaStorage(
            storageOptions,
            metaConnectionString);

        Services.AddBallwareMetaAuthorizationUtils(authorizationOptions.TenantClaim, authorizationOptions.UserIdClaim, authorizationOptions.RightClaim);
        Services.AddBallwareMetaJintRightsChecker();
        Services.AddBallwareMetaApiDependencies();
        Services.AddBallwareMetaBackgroundJobs();

        Services.AddEndpointsApiExplorer();
        
        if (swaggerOptions != null)
        {
            Services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds((type) =>
                {
                    string typeName = type.Name;
                    
                    if (typeName.EndsWith("Dto"))
                    {
                        typeName = typeName.Replace("Dto", string.Empty);
                    }
                    
                    return typeName;
                });
                
                c.SwaggerDoc("meta", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "ballware Meta API",
                    Version = "v1"
                });

                c.SwaggerDoc("service", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "ballware Service API",
                    Version = "v1"
                });

                c.EnableAnnotations();

                c.AddSecurityDefinition("oidc", new OpenApiSecurityScheme
                {
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.OpenIdConnect,
                    OpenIdConnectUrl = new Uri(authorizationOptions.Authority + "/.well-known/openid-configuration")
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oidc" }
                        },
                        swaggerOptions.RequiredScopes.Split(" ")
                    }
                });
            });
        }
    }

    public void InitializeApp(WebApplication app)
    {
        if (Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            IdentityModelEventSource.ShowPII = true;
        }
        
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionFeature?.Error;

                if (exception != null)
                {
                    Log.Error(exception, "Unhandled exception occurred");

                    var problemDetails = new ProblemDetails
                    {
                        Type = "https://httpstatuses.com/500",
                        Title = "An unexpected error occurred.",
                        Status = StatusCodes.Status500InternalServerError,
                        Detail = app.Environment.IsDevelopment() ? exception.ToString() : null,
                        Instance = context.Request.Path
                    };

                    context.Response.StatusCode = problemDetails.Status.Value;
                    context.Response.ContentType = "application/problem+json";
                    await context.Response.WriteAsJsonAsync(problemDetails);
                }
            });
        });

        app.UseCors();
        app.UseRouting();

        app.UseAuthorization();

        app.MapTenantMetaApi("/meta/tenant");
        app.MapTenantServiceApi("/meta/tenant");
        app.MapEditingApi<Data.Public.Tenant>("/meta/tenant", "meta", "tenant", "Tenant", "Tenant");
        
        app.MapSubscriptionMetaApi("/meta/subscription");
        app.MapSubscriptionServiceApi("/meta/subscription");
        app.MapTenantableEditingApi<Subscription>("/meta/subscription", "meta", "subscription", "Subscription", "Subscription");
        
        app.MapStatisticMetaApi("/meta/statistic");
        app.MapStatisticServiceApi("/meta/statistic");
        app.MapTenantableEditingApi<Statistic>("/meta/statistic", "meta", "statistic", "Statistic", "Statistic");
        
        app.MapTenantableEditingApi<EntityRight>("/meta/entityright", "meta", "entityright", "EntityRight", "EntityRight");
        
        app.MapProcessingStateMetaApi("/meta/processingstate");
        app.MapProcessingStateServiceApi("/meta/processingstate");
        app.MapTenantableEditingApi<ProcessingState>("/meta/processingstate", "meta", "processingstate", "Processingstate", "Processingstate");
        
        app.MapPickvalueMetaApi("/meta/pickvalue");
        app.MapPickvalueServiceApi("/meta/pickvalue");
        app.MapTenantableEditingApi<Pickvalue>("/meta/pickvalue", "meta", "pickvalue", "Pickvalue", "Pickvalue");
        
        app.MapPageMetaApi("/meta/page");
        app.MapPageServiceApi("/meta/page");
        app.MapTenantableEditingApi<Page>("/meta/page", "meta", "page", "Page", "Page");
        
        app.MapExportMetaApi("/meta/export");
        app.MapExportServiceApi("/meta/export");
        app.MapTenantableEditingApi<Export>("/meta/export", "meta", "export", "Export", "Export");
        
        app.MapNotificationMetaApi("/meta/notification");
        app.MapNotificationServiceApi("/meta/notification");
        app.MapTenantableEditingApi<Notification>("/meta/notification", "meta", "notification", "Notification", "Notification");
        
        app.MapNotificationTriggerMetaApi("/meta/notificationtrigger");
        app.MapNotificationTriggerServiceApi("/meta/notificationtrigger");
        app.MapTenantableEditingApi<NotificationTrigger>("/meta/notificationtrigger", "meta", "notificationtrigger", "NotificationTrigger", "NotificationTrigger");
        
        app.MapMlModelMetaApi("/meta/mlmodel");
        app.MapMlModelServiceApi("/meta/mlmodel");
        app.MapTenantableEditingApi<MlModel>("/meta/mlmodel", "meta", "mlmodel", "MlModel", "MlModel");
        
        app.MapDocumentationMetaApi("/meta/documentation");
        app.MapDocumentationServiceApi("/meta/documentation");
        app.MapTenantableEditingApi<Documentation>("/meta/documentation", "meta", "documentation", "Documentation", "Documentation");
        
        app.MapJobMetaApi("/meta/job");
        app.MapJobServiceApi("/meta/job");
        app.MapTenantableEditingApi<Job>("/meta/job", "meta", "job", "Job", "Job");

        app.MapLookupMetaApi("/meta/lookup");
        app.MapLookupServiceApi("/meta/lookup");
        app.MapTenantableEditingApi<Lookup>("/meta/lookup", "meta", "lookup", "Lookup", "Lookup");
        
        app.MapDocumentMetaApi("/meta/document");
        app.MapDocumentServiceApi("/meta/document");
        app.MapTenantableEditingApi<Document>("/meta/document", "meta", "document", "Document", "Document");
        
        app.MapEntityMetaApi("/meta/entity");
        app.MapEntityServiceApi("/meta/entity");
        app.MapTenantableEditingApi<EntityMetadata>("/meta/entity", "meta", "entity", "Entity", "Entity");
        
        app.MapSwagger();

        var authorizationOptions = app.Services.GetService<IOptions<AuthorizationOptions>>()?.Value;
        var swaggerOptions = app.Services.GetService<IOptions<SwaggerOptions>>()?.Value;

        if (swaggerOptions != null && authorizationOptions != null)
        {
            app.UseSwagger();

            if (swaggerOptions.EnableClient)
            {
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("meta/swagger.json", "ballware Meta API");
                    c.SwaggerEndpoint("service/swagger.json", "ballware Service API");

                    c.OAuthClientId(swaggerOptions.ClientId);
                    c.OAuthClientSecret(swaggerOptions.ClientSecret);
                    c.OAuthScopes(swaggerOptions.RequiredScopes?.Split(" "));
                    c.OAuthUsePkce();
                });
            }
        }
    }
}