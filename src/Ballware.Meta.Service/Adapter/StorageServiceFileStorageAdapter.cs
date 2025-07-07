using Ballware.Meta.Api;
using Ballware.Meta.Jobs;
using Ballware.Storage.Client;

namespace Ballware.Meta.Service.Adapter;

public class StorageServiceFileStorageAdapter : IMetaFileStorageAdapter, IJobsFileStorageAdapter
{
    private BallwareStorageClient StorageClient { get; }
    
    public StorageServiceFileStorageAdapter(BallwareStorageClient storageClient)
    {
        StorageClient = storageClient;
    }
    
    public async Task<Stream> FileByNameForEntityAndOwnerAsync(Guid tenantId, string entity, Guid ownerId, string fileName)
    {
        var result = await StorageClient.AttachmentDownloadForTenantEntityAndOwnerByFilenameAsync(tenantId, entity, ownerId, fileName);
        
        return result.Stream;
    }

    public async Task RemoveFileByNameForEntityAndOwnerBehalfOfUserAsync(Guid tenantId, Guid userId, string entity, Guid ownerId, string fileName)
    {
        await StorageClient.AttachmentDropForTenantEntityAndOwnerByFilenameBehalfOfUserAsync(tenantId, userId, entity, ownerId, fileName);
    }

    public async Task UploadTemporaryFileBehalfOfUserAsync(Guid tenantId, Guid userId, Guid temporaryId, string fileName, string contentType, Stream data)
    {
        await StorageClient.TemporaryUploadForTenantAndIdBehalfOfUserAsync(tenantId, userId, temporaryId, [new FileParameter(data, fileName, contentType)]);
    }

    public async Task<Stream> TemporaryFileByIdAsync(Guid tenantId, Guid temporaryId)
    {
        var response = await StorageClient.TemporaryDownloadForTenantByIdAsync(tenantId, temporaryId); 
        
        return response.Stream;
    }

    public async Task RemoveTemporaryFileByIdBehalfOfUserAsync(Guid tenantId, Guid userId, Guid temporaryId)
    {
        await StorageClient.TemporaryDropForTenantAndIdBehalfOfUserAsync(tenantId, userId, temporaryId);
    }
}