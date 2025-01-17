using Ballware.Meta.Data.Public;
using Jint;
using Newtonsoft.Json;

namespace Ballware.Meta.Authorization.Jint.Internal;

class JavascriptTenantRightsChecker : ITenantRightsChecker
{
    public async Task<bool> HasRightAsync(Tenant tenant, string application, string entity, Dictionary<string, object> claims, string right)
    {
        var result = true;

        var rightsScript = tenant.RightsCheckScript;

        if (!string.IsNullOrWhiteSpace(rightsScript))
        {
            var userinfo = JsonConvert.SerializeObject(claims);

            result = new Engine()
                .SetValue("right", $"{application}.{entity}.{right}")
                .Execute($"var userinfo = JSON.parse('{userinfo}'); function rightsCheck() {{ {rightsScript} }}")
                .Invoke("rightsCheck").AsBoolean();
        }

        return await Task.FromResult(result);
    }
}