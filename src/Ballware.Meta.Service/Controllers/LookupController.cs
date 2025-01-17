using System;
using System.Net;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Tenant.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ballware.Meta.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LookupController : ControllerBase
{
    private IPrincipalUtils PrincipalUtils { get; }
    private ILookupMetaRepository LookupMetaRepository { get; }
    private ITenantMetaRepository TenantMetaRepository { get; }
    private ITenantLookupProvider TenantLookupProvider { get; }

    public LookupController(IPrincipalUtils principalUtils, ILookupMetaRepository lookupMetaRepository, ITenantMetaRepository tenantMetaRepository, ITenantLookupProvider tenantLookupProvider)
    {
        PrincipalUtils = principalUtils;
        LookupMetaRepository = lookupMetaRepository;
        TenantMetaRepository = tenantMetaRepository;
        TenantLookupProvider = tenantLookupProvider;
    }

    [HttpGet]
    [Route("lookupmetadatabytenantandidentifier/{tenant}/{identifier}")]
    [Authorize("documentApi")]
    [ApiExplorerSettings(GroupName = "document")]
    [SwaggerOperation(
      Summary = "Query lookup metadata by tenant and identifier",
      Description = "",
      OperationId = "MetadataForLookupByTenantAndIdentifier"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Lookup metadata", typeof(Lookup), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> LookupMetadataByTenantAndId(Guid tenant, string identifier)
    {
        try
        {
            var lookup = await LookupMetaRepository.ByIdentifierAsync(tenant, identifier);

            if (lookup == null)
            {
                return NotFound();
            }

            return Ok(lookup);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpGet]
    [Route("selectlistforlookup/{id}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query entries for lookup by id",
      Description = "",
      OperationId = "ValuesForLookupById"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "List of lookup entries", typeof(IEnumerable<object>), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> SelectListForLookup(Guid id)
    {
        try
        {
            var tenantId = PrincipalUtils.GetUserTenandId(User);
            var rights = PrincipalUtils.GetUserRights(User);

            var tenant = await TenantMetaRepository.ByIdAsync(tenantId);
            var lookup = await LookupMetaRepository.ByIdAsync(tenantId, id);

            if (tenant == null || lookup == null)
            {
                return NotFound();
            }

            return Ok(await TenantLookupProvider.SelectListForLookupAsync(tenant, lookup, rights));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpGet]
    [Route("selectbyidforlookup/{lookup}/{id}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query single entry for lookup by id",
      Description = "",
      OperationId = "ValueForLookupById"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "List of lookup entries", typeof(object), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> SelectByIdForLookup(Guid lookupId, string id)
    {
        try
        {
            var tenantId = PrincipalUtils.GetUserTenandId(User);
            var rights = PrincipalUtils.GetUserRights(User);

            var tenant = await TenantMetaRepository.ByIdAsync(tenantId);
            var lookup = await LookupMetaRepository.ByIdAsync(tenantId, lookupId);

            if (tenant == null || lookup == null)
            {
                return NotFound();
            }

            return Ok(await TenantLookupProvider.SelectByIdForLookupAsync(tenant, lookup, id, rights));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpGet]
    [Route("selectlistforlookupidentifier/{identifier}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query entries for lookup by identifier",
      Description = "",
      OperationId = "ValuesForLookupByIdentifier"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "List of lookup entries", typeof(IEnumerable<object>), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> SelectListForLookupIdentifier(string identifier)
    {
        try
        {
            var tenantId = PrincipalUtils.GetUserTenandId(User);
            var rights = PrincipalUtils.GetUserRights(User);

            var tenant = await TenantMetaRepository.ByIdAsync(tenantId);
            var lookup = await LookupMetaRepository.ByIdentifierAsync(tenantId, identifier);

            if (tenant == null || lookup == null)
            {
                return NotFound();
            }

            return Ok(await TenantLookupProvider.SelectListForLookupAsync(tenant, lookup, rights));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpGet]
    [Route("selectbyidforlookupidentifier/{identifier}/{id}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query single entry for lookup by identifier",
      Description = "",
      OperationId = "ValueForLookupByIdentifier"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Lookup entry", typeof(object), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> SelectByIdForLookupIdentifier(string identifier, string id)
    {
        try
        {
            var tenantId = PrincipalUtils.GetUserTenandId(User);
            var rights = PrincipalUtils.GetUserRights(User);

            var tenant = await TenantMetaRepository.ByIdAsync(tenantId);
            var lookup = await LookupMetaRepository.ByIdentifierAsync(tenantId, identifier);

            if (tenant == null || lookup == null)
            {
                return NotFound();
            }

            return Ok(await TenantLookupProvider.SelectByIdForLookupAsync(tenant, lookup, id, rights));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpGet]
    [Route("selectlistforlookupwithparam/{id}/{param}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query entry for lookup with param by id",
      Description = "",
      OperationId = "ValuesForLookupWithParamById"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "List of lookup entries", typeof(IEnumerable<object>), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> SelectListForLookupWithParam(Guid id, string param)
    {
        try
        {
            var tenantId = PrincipalUtils.GetUserTenandId(User);
            var rights = PrincipalUtils.GetUserRights(User);

            var tenant = await TenantMetaRepository.ByIdAsync(tenantId);
            var lookup = await LookupMetaRepository.ByIdAsync(tenantId, id);

            if (tenant == null || lookup == null)
            {
                return NotFound();
            }

            return Ok(await TenantLookupProvider.SelectListForLookupWithParamAsync(tenant, lookup, rights, param));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpGet]
    [Route("selectbyidforlookupwithparam/{lookup}/{param}/{id}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query single entry for lookup with param by id",
      Description = "",
      OperationId = "ValueForLookupWithParamById"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Lookup entry", typeof(object), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> SelectByIdForLookupWithParam(Guid lookupId, string param, string id)
    {
        try
        {
            var tenantId = PrincipalUtils.GetUserTenandId(User);
            var rights = PrincipalUtils.GetUserRights(User);

            var tenant = await TenantMetaRepository.ByIdAsync(tenantId);
            var lookup = await LookupMetaRepository.ByIdAsync(tenantId, lookupId);

            if (tenant == null || lookup == null)
            {
                return NotFound();
            }

            return Ok(await TenantLookupProvider.SelectByIdForLookupWithParamAsync(tenant, lookup, rights, param, id));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpGet]
    [Route("autocompleteforlookup/{id}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query autocomplete entries for lookup by id",
      Description = "",
      OperationId = "AutocompleteValuesForLookupById"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "List of autocomplete entries", typeof(IEnumerable<string>), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> AutoCompleteForLookup(Guid id)
    {
        try
        {
            var tenantId = PrincipalUtils.GetUserTenandId(User);
            var rights = PrincipalUtils.GetUserRights(User);

            var tenant = await TenantMetaRepository.ByIdAsync(tenantId);
            var lookup = await LookupMetaRepository.ByIdAsync(tenantId, id);

            if (tenant == null || lookup == null)
            {
                return NotFound();
            }

            return Ok(await TenantLookupProvider.AutoCompleteForLookupAsync(tenant, lookup, rights));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }

    [HttpGet]
    [Route("autocompleteforlookupwithparam/{id}/{param}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query autocomplete entries for lookup with param by id",
      Description = "",
      OperationId = "AutocompleteValuesForLookupWithParamById"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "List of autocomplete entries", typeof(IEnumerable<string>), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> AutoCompleteForLookupWithParam(Guid id, string param)
    {
        try
        {
            var tenantId = PrincipalUtils.GetUserTenandId(User);
            var rights = PrincipalUtils.GetUserRights(User);

            var tenant = await TenantMetaRepository.ByIdAsync(tenantId);
            var lookup = await LookupMetaRepository.ByIdAsync(tenantId, id);

            if (tenant == null || lookup == null)
            {
                return NotFound();
            }

            return Ok(await TenantLookupProvider.AutoCompleteForLookupWithParamAsync(tenant, lookup, rights, param));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex);
        }
    }
}