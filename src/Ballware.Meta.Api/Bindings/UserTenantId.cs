using System.Reflection;
using Ballware.Meta.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Api.Bindings;

public class UserTenantId
{
    public Guid Value { get; private set; }
    
    public static ValueTask<UserTenantId> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var principalUtils = context.RequestServices.GetRequiredService<IPrincipalUtils>();
        
        if (context.User.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        return ValueTask.FromResult(new UserTenantId() { Value = principalUtils.GetUserTenandId(context.User) });
    }
}