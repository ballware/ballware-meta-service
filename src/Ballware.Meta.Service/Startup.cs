using Ballware.Meta.Authorization;
using Ballware.Meta.Authorization.Jint;
using Ballware.Meta.Data.Ef;
using Ballware.Meta.Data.Ef.Configuration;
using Ballware.Meta.Service.Configuration;
using Ballware.Meta.Service.Mappings;
using Ballware.Meta.Tenant.Data;
using Ballware.Meta.Tenant.Data.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;

namespace Ballware.Meta.Service;


public class Startup(IWebHostEnvironment environment, ConfigurationManager configuration, IServiceCollection services)
{
    private IWebHostEnvironment Environment { get; } = environment;
    private ConfigurationManager Configuration { get; } = configuration;
    private IServiceCollection Services { get; } = services;

    public void InitializeServices()
    {
        CorsOptions? corsOptions = Configuration.GetSection("Cors").Get<CorsOptions>();
        AuthorizationOptions? authorizationOptions =
            Configuration.GetSection("Authorization").Get<AuthorizationOptions>();
        StorageOptions? storageOptions = Configuration.GetSection("Storage").Get<StorageOptions>();
        SwaggerOptions? swaggerOptions = Configuration.GetSection("Swagger").Get<SwaggerOptions>();
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

        if (authorizationOptions == null || storageOptions == null || string.IsNullOrEmpty(metaConnectionString))
        {
            throw new ConfigurationException("Required configuration for authorization and storage is missing");
        }

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

        Services.AddAutoMapper(config =>
        {
            config.AddBallwareStorageMappings();
            config.AddProfile<MetaApiProfile>();
            config.AddProfile<ServiceApiProfile>();
        });

        Services.AddBallwareMetaStorage(
            storageOptions,
            metaConnectionString);

        Services.AddBallwareTenantStorage(builder =>
        {
            builder.AddSqlServerTenantDataStorage();
        });

        Services.AddBallwareAuthorizationUtils(authorizationOptions.TenantClaim, authorizationOptions.UserIdClaim, authorizationOptions.RightClaim);
        Services.AddBallwareJintRightsChecker();

        if (swaggerOptions != null)
        {
            Services.AddSwaggerGen(c =>
            {
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

        app.MapControllers();

        var authorizationOptions = app.Services.GetService<IOptions<AuthorizationOptions>>()?.Value;
        var swaggerOptions = app.Services.GetService<IOptions<SwaggerOptions>>()?.Value;

        if (swaggerOptions != null && authorizationOptions != null)
        {
            app.MapSwagger();

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