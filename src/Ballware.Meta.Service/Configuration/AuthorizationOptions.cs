using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;

namespace Ballware.Meta.Service.Configuration;

public class AuthorizationOptions
{
    [Required]
    public required string Authority { get; set; }

    [Required]
    public required string Audience { get; set; }

    [Required]
    public required string TenantClaim { get; set; } = "tenant";

    [Required] public required string UserIdClaim { get; set; } = JwtRegisteredClaimNames.Sub;
    
    public bool RequireHttpsMetadata { get; set; } = true;
    public string RequiredMetaScope { get; set; } = "metaApi";
    public string RequiredServiceScope { get; set; } = "serviceApi";
}