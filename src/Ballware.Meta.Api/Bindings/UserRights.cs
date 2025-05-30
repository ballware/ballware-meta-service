using System.Reflection;
using Ballware.Meta.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Ballware.Meta.Api.Bindings;

public class UserRights
{
    public IEnumerable<string> Value { get; private set; } = null!;
    
    public static ValueTask<UserRights> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var principalUtils = context.RequestServices.GetRequiredService<IPrincipalUtils>();
        
        if (context.User.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        return ValueTask.FromResult(new UserRights() { Value = principalUtils.GetUserRights(context.User) });
    }
}