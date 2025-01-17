using System;
using System.Collections.Immutable;
using System.Net;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data.Public;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ballware.Meta.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DocumentController : ControllerBase
{
    private IPrincipalUtils PrincipalUtils { get; }
    private IDocumentMetaRepository MetaRepository { get; }
    private ITenantMetaRepository TenantMetaRepository { get; }
    private ITenantRightsChecker TenantRightsChecker { get; }

    public DocumentController(IPrincipalUtils princialUtils, IDocumentMetaRepository metaRepository, ITenantMetaRepository tenantMetaRepository, ITenantRightsChecker tenantRightsChecker)
    {
        PrincipalUtils = princialUtils;
        MetaRepository = metaRepository;
        TenantMetaRepository = tenantMetaRepository;
        TenantRightsChecker = tenantRightsChecker;
    }

    [HttpGet]
    [Route("selectlistdocumentsforentity/{entity}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query available documents for entity",
      Description = "",
      OperationId = "DocumentsForEntity"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.OK, "List of available documents for user", typeof(IEnumerable<DocumentSelectListEntry>), new[] { MimeMapping.KnownMimeTypes.Json })]
    public virtual async Task<IActionResult> SelectListPrintDocumentsForEntity(string entity)
    {
        var tenantId = PrincipalUtils.GetUserTenandId(User);
        var claims = PrincipalUtils.GetUserClaims(User);

        var tenantMeta = await TenantMetaRepository.ByIdAsync(tenantId);

        if (tenantMeta == null)
        {
            return NotFound();
        }

        var documentList = (await MetaRepository.SelectListForTenantAndEntityAsync(tenantId, entity))
                .ToAsyncEnumerable()
                .WhereAwait(async d =>
                    await TenantRightsChecker.HasRightAsync(tenantMeta, "meta", "document", claims,
                        $"visiblestate.{d.State}"))
                .ToEnumerable();

        return Ok(documentList);
    }

    [HttpGet]
    [Route("selectlistdocumentsfortenant/{tenant}")]
    [ApiExplorerSettings(GroupName = "service")]
    [Authorize("serviceApi")]
    [SwaggerOperation(
      Summary = "Query available documents for tenant",
      Description = "",
      OperationId = "DocumentsForTenant"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "List of available documents for tenant", typeof(IEnumerable<DocumentSelectListEntry>), new[] { MimeMapping.KnownMimeTypes.Json })]
    public virtual async Task<IActionResult> SelectListPrintDocumentsForTenant(Guid tenant)
    {
        return Ok(await MetaRepository.SelectListForTenantAsync(tenant));
    }

    [HttpGet]
    [Route("documentmetadatabytenantandid/{tenant}/{id}")]
    [ApiExplorerSettings(GroupName = "service")]
    [Authorize("serviceApi")]
    [SwaggerOperation(
      Summary = "Query document metadata by tenant and id",
      Description = "",
      OperationId = "MetadataForDocumentByTenantAndId"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Document metadata", typeof(Document), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> DocumentMetadataByTenantAndId(Guid tenant, Guid id)
    {
        var document = await MetaRepository.MetadataByTenantAndIdAsync(tenant, id);

        return Ok(document);
    }

    [HttpPost]
    [Route("savedocumentbehalfofuser/{tenant}/{user}")]
    [ApiExplorerSettings(GroupName = "service")]
    [Authorize("serviceApi")]
    [SwaggerOperation(
      Summary = "Save document behalf of user",
      Description = "",
      OperationId = "SaveDocumentBehalfOfUser"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Save Document")]
    public async Task<IActionResult> SaveDocumentBehalfOfuser(Guid tenant, Guid user, [FromBody] Document document)
    {
        await MetaRepository.SaveAsync(tenant, user, "primary", ImmutableDictionary<string, object>.Empty, document);

        return Ok();
    }

    [HttpGet]
    [Route("documenttemplatebehalfofuserbytenant/{tenant}/{user}")]
    [ApiExplorerSettings(GroupName = "service")]
    [Authorize("serviceApi")]
    [SwaggerOperation(
      Summary = "Query new document template by tenant behalf of user",
      Description = "",
      OperationId = "DocumentTemplateBehalfOfUserByTenant"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Document template", typeof(Document), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> DocumentTemplateByTenant(Guid tenant, Guid user)
    {
        var document = await MetaRepository.NewAsync(tenant, "primary", ImmutableDictionary<string, object>.Empty);

        return Ok(document);
    }
}