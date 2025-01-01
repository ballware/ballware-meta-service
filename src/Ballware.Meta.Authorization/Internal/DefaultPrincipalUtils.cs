using System.Security.Claims;

namespace Ballware.Meta.Authorization.Internal;

class DefaultPrincipalUtils : IPrincipalUtils
{
    private string TenantClaim { get; }
    private string UserIdClaim { get; }
    
    public DefaultPrincipalUtils(string tenantClaim, string userIdClaim)
    {
        TenantClaim = tenantClaim;
        UserIdClaim = userIdClaim;
    }

    public Guid GetUserId(ClaimsPrincipal principal)
    {
        var userIdClaimValue = principal.Claims.FirstOrDefault(c => c.Type.Equals(UserIdClaim));
        
        Guid.TryParse(userIdClaimValue?.Value, out var userId);
        
        return userId;
    } 
    
    public Guid GetUserTenandId(ClaimsPrincipal principal)
    {
        var tenantClaimValue = principal.Claims.FirstOrDefault(c => c.Type.Equals(TenantClaim));
        
        Guid.TryParse(tenantClaimValue?.Value, out var tenantId);
        
        return tenantId;
    }

    public Dictionary<string, object> GetUserClaims(ClaimsPrincipal principal)
    {
        var userinfoTemp = new Dictionary<string, List<string>>();

        foreach (var cl in principal.Claims) {
            if (userinfoTemp.ContainsKey(cl.Type)) {
                userinfoTemp[cl.Type].Add(cl.Value);
            } else {
                userinfoTemp.Add(cl.Type, new List<string>(new [] { cl.Value }));
            }
        }

        var claims = userinfoTemp.Select(cl => {
            if (cl.Value.Count > 1) {
                return new KeyValuePair<string, object>(cl.Key, cl.Value.ToArray());
            } else {
                return new KeyValuePair<string, object>(cl.Key, cl.Value[0]);
            }
        }).ToDictionary(elem => elem.Key, elem => elem.Value);
        
        return claims;
    }
}