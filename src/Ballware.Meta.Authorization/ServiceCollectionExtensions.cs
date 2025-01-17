using System.Diagnostics.CodeAnalysis;
using Ballware.Meta.Authorization.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Authorization;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBallwareAuthorizationUtils(this IServiceCollection services, string tenantClaim, string userIdClaim, string rightClaim)
    {
        services.AddSingleton<IPrincipalUtils>(new DefaultPrincipalUtils(tenantClaim, userIdClaim, rightClaim));

        return services;
    }
}