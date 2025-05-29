using System.Text.Json;
using Ballware.Meta.Data.Public;
using Jint;

namespace Ballware.Meta.Authorization.Jint.Internal;

class JavascriptEntityRightsChecker : IEntityRightsChecker
{
    public async Task<bool> HasRightAsync(Guid tenantId, EntityMetadata metadata, IDictionary<string, object> claims, string right, object param,
        bool tenantResult)
    {
        var result = tenantResult;
        var rightsScript = metadata.CustomScripts?.GetCustomScripts()?.ExtendedRightsCheck;

        if (!string.IsNullOrWhiteSpace(rightsScript))
        {
            var userinfo = JsonSerializer.Serialize(claims);

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