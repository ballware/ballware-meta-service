using System.Net;
using Ballware.Meta.Authorization;
using Ballware.Meta.Data;
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
[Authorize("metaApi", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TenantController : ControllerBase
{
    private IPrincipalUtils PrincipalUtils { get; }
    private IMetaDbConnectionFactory MetaConnectionFactory { get; }
    private ITenantStorageProvider TenantStorageProvider { get; }
    private ITenantMetaRepository TenantMetaRepository { get; }
    private ILookupMetaRepository LookupMetaRepository { get; }
    private IPickvalueMetaRepository PickvalueMetaRepository { get; }
    private IProcessingStateMetaRepository ProcessingStateMetaRepository { get; }
    private IEntityMetaRepository EntityMetaRepository { get; }

    public TenantController(
        IPrincipalUtils principalUtils,
        IMetaDbConnectionFactory metaConnectionFactory,
        ITenantStorageProvider tenantStorageProvider,
        ITenantMetaRepository tenantMetaRepository,
        ILookupMetaRepository lookupMetaMetaRepository,
        IPickvalueMetaRepository pickvalueMetaRepository,
        IProcessingStateMetaRepository processingStateMetaRepository,
        IEntityMetaRepository entityRepository)
    {
        PrincipalUtils = principalUtils;
        MetaConnectionFactory = metaConnectionFactory;
        TenantStorageProvider = tenantStorageProvider;
        TenantMetaRepository = tenantMetaRepository;
        LookupMetaRepository = lookupMetaMetaRepository;
        PickvalueMetaRepository = pickvalueMetaRepository;
        ProcessingStateMetaRepository = processingStateMetaRepository;
        EntityMetaRepository = entityRepository;
    }

    [HttpGet]
    [Route("metadatafortenant/{tenant}")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query metadata for tenant by id",
      Description = "",
      OperationId = "MetadataForTenantById"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "Tenant metadata", typeof(Data.Public.Tenant), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> MetadataForTenant(Guid tenant)
    {
        var tenantId = PrincipalUtils.GetUserTenandId(User);

        if (tenantId != tenant)
            return Forbid();

        return Ok(await TenantMetaRepository.ByIdAsync(tenant));
    }

    [HttpGet]
    [Route("reportdatasourcesfortenant/{tenant}")]
    [Authorize("documentApi")]
    [ApiExplorerSettings(GroupName = "document")]
    [SwaggerOperation(
      Summary = "Query report datasources for tenant by id",
      Description = "",
      OperationId = "ReportDatasoucesForTenantById"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.NotFound)]
    [SwaggerResponse((int)HttpStatusCode.OK, "List of report datasources", typeof(IEnumerable<ReportDatasourceDefinition>), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> ReportSchemaForTenant(Guid tenant)
    {
        var tenantObject = await TenantMetaRepository.ByIdAsync(tenant);

        if (tenantObject == null)
        {
            return NotFound();
        }

        var metaConnectionString = MetaConnectionFactory.ConnectionString;
        var tenantConnectionString = TenantStorageProvider.GetConnectionString(tenantObject);

        var schemaDefinitions = new List<ReportDatasourceDefinition>();
        
        schemaDefinitions.Add(await CreateMetaSchemaDefinitionAsync(tenantObject, metaConnectionString));
        schemaDefinitions.Add(await CreateLookupSchemaDefinitionAsync(tenantObject, tenantConnectionString));
        schemaDefinitions.Add(await CreateMetaLookupSchemaDefinitionAsync(tenantObject, metaConnectionString));
        schemaDefinitions.AddRange(await CreateTenantSchemaDefinitionsAsync(tenantObject, tenantConnectionString));
        
        return Ok(schemaDefinitions);
    }

    [HttpGet]
    [Route("allowed")]
    [ApiExplorerSettings(GroupName = "meta")]
    [SwaggerOperation(
      Summary = "Query list of allowed tenants for authenticated user",
      Description = "",
      OperationId = "AllowedTenantsForUser"
    )]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized)]
    [SwaggerResponse((int)HttpStatusCode.OK, "List of allowed tenants", typeof(IEnumerable<Data.SelectLists.TenantSelectListEntry>), new[] { MimeMapping.KnownMimeTypes.Json })]
    public async Task<IActionResult> AllowedTenantsForUser()
    {
        var claims = PrincipalUtils.GetUserClaims(User);

        return Ok(await TenantMetaRepository.AllowedTenantsAsync(claims));
    }

    private async Task<ReportDatasourceDefinition> CreateMetaSchemaDefinitionAsync(Data.Public.Tenant tenant,
        string metaConnectionString)
    {
        return new ReportDatasourceDefinition
        {
            Name = "Meta",
            ConnectionString = metaConnectionString,
            Tables = new[]
            {
                new ReportDatasourceTable
                    { Name = "Pickvalue", Query = await PickvalueMetaRepository.GenerateListQueryAsync(tenant.Id) },
                new ReportDatasourceTable
                {
                    Name = "ProcessingState",
                    Query = await ProcessingStateMetaRepository.GenerateListQueryAsync(tenant.Id)
                }
            }
        };
    } 
    
    private async Task<ReportDatasourceDefinition> CreateLookupSchemaDefinitionAsync(Data.Public.Tenant tenant, string tenantConnectionString)
    {
        return new ReportDatasourceDefinition
        {
            Name = "Lookups",
            ConnectionString = tenantConnectionString,
            Tables = (await LookupMetaRepository.AllForTenantAsync(tenant.Id))
                .Where(l => !l.Meta && !l.HasParam)
                .Select(l => new ReportDatasourceTable
                {
                    Name = l.Identifier,
                    Query = !string.IsNullOrEmpty(l.ListQuery) ? TenantStorageProvider.ApplyTenantPlaceholder(tenant, l.ListQuery, TenantPlaceholderOptions.Create().WithReplaceTenantId().WithReplaceClaims()) : l.ListQuery
                })
        };
    }

    private async Task<ReportDatasourceDefinition> CreateMetaLookupSchemaDefinitionAsync(Data.Public.Tenant tenant, string metaConnectionString)
    {
        return new ReportDatasourceDefinition
        {
            Name = "MetaLookups",
            ConnectionString = metaConnectionString,
            Tables = (await LookupMetaRepository.AllForTenantAsync(tenant.Id))
                .Where(l => l.Meta && !l.HasParam)
                .Select(l => new ReportDatasourceTable
                {
                    Name = l.Identifier,
                    Query = !string.IsNullOrEmpty(l.ListQuery) 
                        ? TenantStorageProvider.ApplyTenantPlaceholder(tenant, l.ListQuery, 
                            TenantPlaceholderOptions.Create()
                                .WithReplaceTenantId()
                                .WithReplaceClaims()) 
                        : l.ListQuery
                })
        };
    }

    private async Task<IEnumerable<ReportDatasourceDefinition>> CreateTenantSchemaDefinitionsAsync(Data.Public.Tenant tenant,
        string tenantConnectionString)
    {
        var result = new List<ReportDatasourceDefinition>();
        
        var tenantSchemaDefinitions = tenant.ReportSchemaDefinition?.ToReportSchemaDefinition();

        foreach (var schemaDefinition in tenantSchemaDefinitions ?? new List<ReportDatasourceDefinition>())
        {
            schemaDefinition.ConnectionString = tenantConnectionString;

            foreach (var table in schemaDefinition.Tables ?? new List<ReportDatasourceTable>())
            {
                if (!string.IsNullOrEmpty(table.Entity))
                {
                    var entityMeta = await EntityMetaRepository.ByEntityAsync(tenant.Id, table.Entity);

                    if (entityMeta != null)
                    {
                        var query = entityMeta.ListQuery?.GetQueryByIdentifier(table.Query ?? "primary")?.Query;

                        if (query != null)
                        {
                            table.Query = await TenantStorageProvider
                                .ApplyTenantPlaceholderAsync(tenant, query,
                                    TenantPlaceholderOptions.Create()
                                        .WithReplaceTenantId());
                        }
                    }

                }
            }

            result.Add(schemaDefinition);
        }
        
        return result;
    }
}