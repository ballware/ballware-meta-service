using Ballware.Meta.Api;
using Ballware.Meta.Api.Endpoints;
using Ballware.Meta.Authorization;
using Ballware.Meta.Authorization.Jint;
using Ballware.Meta.Data.Ef;
using Ballware.Meta.Data.Ef.Configuration;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Service.Adapter;
using Ballware.Meta.Service.Configuration;
using Ballware.Meta.Service.Extensions;
using Ballware.Meta.Service.Jobs;
using Ballware.Schema.Client;
using Ballware.Storage.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Quartz;
using Quartz.AspNetCore;

namespace Ballware.Meta.Service;


public class Startup(IWebHostEnvironment environment, ConfigurationManager configuration, IServiceCollection services)
{
    private IWebHostEnvironment Environment { get; } = environment;
    private ConfigurationManager Configuration { get; } = configuration;
    private IServiceCollection Services { get; } = services;

    public void InitializeServices()
    {
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = null; // = PascalCase
        });
        
        CorsOptions? corsOptions = Configuration.GetSection("Cors").Get<CorsOptions>();
        AuthorizationOptions? authorizationOptions =
            Configuration.GetSection("Authorization").Get<AuthorizationOptions>();
        StorageOptions? storageOptions = Configuration.GetSection("Storage").Get<StorageOptions>();
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
        
        if (storageClientOptions == null)
        {
            throw new ConfigurationException("Required configuration for storageClient is missing");
        }
        
        if (schemaClientOptions == null)
        {
            throw new ConfigurationException("Required configuration for schemaClient is missing");
        }

        Services.AddLogging();
        Services.AddMemoryCache();
        Services.AddDistributedMemoryCache();
        
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
            .AddNewtonsoftJson(opts => opts.SerializerSettings.ContractResolver = new DefaultContractResolver())
            .AddApiExplorer();

        Services.AddControllers()
            .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null)
            .AddNewtonsoftJson(opts => opts.SerializerSettings.ContractResolver = new DefaultContractResolver());
        
        Services.Configure<QuartzOptions>(Configuration.GetSection("Quartz"));
        Services.AddQuartz(q =>
        {
            q.AddJob<TenantImportJob>(new JobKey("import", "tenant"), configurator => configurator.StoreDurably());
                
            q.AddJob<MetaImportJob<Documentation, ITenantableRepository<Documentation>>>(new JobKey("import", "documentation"), configurator => configurator.StoreDurably());
            q.AddJob<MetaImportJob<Document, ITenantableRepository<Document>>>(new JobKey("import", "document"), configurator => configurator.StoreDurably());
            q.AddJob<MetaImportJob<EntityMetadata, ITenantableRepository<EntityMetadata>>>(new JobKey("import", "entity"), configurator => configurator.StoreDurably());
            q.AddJob<MetaImportJob<Lookup, ITenantableRepository<Lookup>>>(new JobKey("import", "lookup"), configurator => configurator.StoreDurably());
            q.AddJob<MetaImportJob<MlModel, ITenantableRepository<MlModel>>>(new JobKey("import", "mlmodel"), configurator => configurator.StoreDurably());
            q.AddJob<MetaImportJob<Notification, ITenantableRepository<Notification>>>(new JobKey("import", "notification"), configurator => configurator.StoreDurably());
            q.AddJob<MetaImportJob<Page, ITenantableRepository<Page>>>(new JobKey("import", "page"), configurator => configurator.StoreDurably());
            q.AddJob<MetaImportJob<Statistic, ITenantableRepository<Statistic>>>(new JobKey("import", "statistic"), configurator => configurator.StoreDurably());
            q.AddJob<MetaImportJob<Subscription, ITenantableRepository<Subscription>>>(new JobKey("import", "subscription"), configurator => configurator.StoreDurably());
        });

        Services.AddQuartzServer(options =>
        {
            options.WaitForJobsToComplete = true;
        });
        
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
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddClientCredentialsTokenHandler("schema");
        
        Services.AddAutoMapper(config =>
        {
            config.AddBallwareStorageMappings();
            config.AddBallwareMetaApiMappings();
        });
        
        Services.AddScoped<IMetaFileStorageAdapter, StorageServiceMetaFileStorageAdapter>();
        Services.AddScoped<IRepositoryHook<Ballware.Meta.Data.Public.Tenant, Ballware.Meta.Data.Persistables.Tenant>, GenericSchemaTenantRepositoryHook>();
        Services.AddScoped<ITenantableRepositoryHook<Ballware.Meta.Data.Public.EntityMetadata, Ballware.Meta.Data.Persistables.EntityMetadata>, GenericSchemaEntityRepositoryHook>();
        
        Services.AddBallwareMetaStorage(
            storageOptions,
            metaConnectionString);

        Services.AddBallwareMetaAuthorizationUtils(authorizationOptions.TenantClaim, authorizationOptions.UserIdClaim, authorizationOptions.RightClaim);
        Services.AddBallwareMetaJintRightsChecker();

        services.AddEndpointsApiExplorer();
        
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

            Services.AddSwaggerGenNewtonsoftSupport();
        }
    }

    public void InitializeApp(WebApplication app)
    {
        if (Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            IdentityModelEventSource.ShowPII = true;
        }

        app.UseCors();
        app.UseRouting();

        app.UseAuthorization();

        app.MapTenantMetaApi("/api/tenant");
        app.MapTenantServiceApi("/api/tenant");
        app.MapEditingApi<Data.Public.Tenant>("/api/tenant", "meta", "tenant", "Tenant", "Tenant");
        
        app.MapSubscriptionMetaApi("/api/subscription");
        app.MapSubscriptionServiceApi("/api/subscription");
        app.MapTenantableEditingApi<Subscription>("/api/subscription", "meta", "subscription", "Subscription", "Subscription");
        
        app.MapStatisticMetaApi("/api/statistic");
        app.MapStatisticServiceApi("/api/statistic");
        app.MapTenantableEditingApi<Statistic>("/api/statistic", "meta", "statistic", "Statistic", "Statistic");
        
        app.MapProcessingStateMetaApi("/api/processingstate");
        app.MapProcessingStateServiceApi("/api/processingstate");
        
        app.MapPickvalueMetaApi("/api/pickvalue");
        app.MapPickvalueServiceApi("/api/pickvalue");
        
        app.MapPageMetaApi("/api/page");
        app.MapPageServiceApi("/api/page");
        app.MapTenantableEditingApi<Page>("/api/page", "meta", "page", "Page", "Page");
        
        app.MapExportMetaApi("/api/export");
        app.MapExportServiceApi("/api/export");
        app.MapTenantableEditingApi<Export>("/api/export", "meta", "export", "Export", "Export");
        
        app.MapNotificationMetaApi("/api/notification");
        app.MapNotificationServiceApi("/api/notification");
        app.MapTenantableEditingApi<Notification>("/api/notification", "meta", "notification", "Notification", "Notification");
        
        app.MapNotificationTriggerMetaApi("/api/notificationtrigger");
        app.MapNotificationTriggerServiceApi("/api/notificationtrigger");
        app.MapTenantableEditingApi<NotificationTrigger>("/api/notificationtrigger", "meta", "notificationtrigger", "NotificationTrigger", "NotificationTrigger");
        
        app.MapMlModelMetaApi("/api/mlmodel");
        app.MapMlModelServiceApi("/api/mlmodel");
        app.MapTenantableEditingApi<MlModel>("/api/mlmodel", "meta", "mlmodel", "MlModel", "MlModel");
        
        app.MapDocumentationMetaApi("/api/documentation");
        app.MapDocumentationServiceApi("/api/documentation");
        app.MapTenantableEditingApi<Documentation>("/api/documentation", "meta", "documentation", "Documentation", "Documentation");
        
        app.MapJobMetaApi("/api/job");
        app.MapJobServiceApi("/api/job");
        app.MapTenantableEditingApi<Job>("/api/job", "meta", "job", "Job", "Job");

        app.MapLookupMetaApi("/api/lookup");
        app.MapLookupServiceApi("/api/lookup");
        app.MapTenantableEditingApi<Lookup>("/api/lookup", "meta", "lookup", "Lookup", "Lookup");
        
        app.MapDocumentMetaApi("/api/document");
        app.MapDocumentServiceApi("/api/document");
        app.MapTenantableEditingApi<Document>("/api/document", "meta", "document", "Document", "Document");
        
        app.MapEntityMetaApi("/api/entity");
        app.MapEntityServiceApi("/api/entity");
        app.MapTenantableEditingApi<EntityMetadata>("/api/entity", "meta", "entity", "Entity", "Entity");
        
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