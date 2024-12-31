using Ballware.Meta.Data;
using Jint;
using Newtonsoft.Json;

namespace Ballware.Meta.Authorization.Jint.Internal;

class JavascriptEntityRightsChecker : IEntityRightsChecker
{
    public async Task<bool> HasRightAsync(Guid tenantId, EntityMetadata metadata, Dictionary<string, object> claims, string right, IDictionary<string, object> param,
        bool tenantResult)
    {
        var result = tenantResult;
        var rightsScript = metadata.CustomScripts?.GetCustomScripts()?.ExtendedRightsCheck;

        if (!string.IsNullOrWhiteSpace(rightsScript))
        {
            var userinfo = JsonConvert.SerializeObject(claims);

            result = bool.Parse(new Engine()
                .SetValue("application", metadata.Application)
                .SetValue("entity", metadata.Entity)
                .SetValue("right", right)
                .SetValue("param", param)
                .SetValue("result", tenantResult)
                .Evaluate($"var userinfo = JSON.parse('{userinfo}'); function extendedRightsCheck() {{ {rightsScript} }} return extendedRightsCheck();")
                .ToString());
        }

        return await Task.FromResult(result);
    }
}