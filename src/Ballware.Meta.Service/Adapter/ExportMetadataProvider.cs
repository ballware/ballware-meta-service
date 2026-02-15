using System.Collections.Immutable;
using Ballware.Meta.Data.Repository;
using Ballware.Shared.Api;
using Ballware.Shared.Api.Public;

namespace Ballware.Meta.Service.Adapter;

public class ExportMetadataProvider(IExportMetaRepository exportMetaRepository) : IExportMetadataProvider
{
    public async Task<Guid> CreateExportAsync(Guid tenantId, Guid userId, Export payload)
    {
        var export = await exportMetaRepository.NewAsync(tenantId, "primary", ImmutableDictionary<string, object>.Empty);
        
        export.Application = payload.Application;
        export.Entity = payload.Entity;
        export.Query = payload.Query;
        export.ExpirationStamp = payload.ExpirationStamp;
        export.MediaType = payload.MediaType;
    
        await exportMetaRepository.SaveAsync(tenantId, userId, "primary", ImmutableDictionary<string, object>.Empty, export);

        return export.Id;
    }

    public async Task<Export?> GetExportByIdAsync(Guid tenantId, Guid exportId)
    {
        var export = await exportMetaRepository.ByIdAsync(tenantId, exportId);

        if (export != null)
        {
            return new Shared.Api.Public.Export()
            {
                Id = export.Id,
                Application = export.Application,
                Entity = export.Entity,
                Query = export.Query,
                ExpirationStamp = export.ExpirationStamp,
                MediaType = export.MediaType,
            };    
        }

        return null;
    }
}