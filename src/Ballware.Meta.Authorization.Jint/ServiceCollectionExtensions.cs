using Ballware.Meta.Authorization.Jint.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Authorization.Jint;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBallwareJintRightsChecker(this IServiceCollection services)
    {
        services.AddSingleton<ITenantRightsChecker, JavascriptTenantRightsChecker>();
        services.AddSingleton<IEntityRightsChecker, JavascriptEntityRightsChecker>();
        
        return services;
    }

}