using Ballware.Meta.Api;
using Ballware.Meta.Jobs;
using Ballware.Storage.Service.Client;

namespace Ballware.Meta.Service.Adapter;

public class StorageServiceFileStorageAdapter : IMetaFileStorageAdapter, IJobsFileStorageAdapter
{
    private StorageServiceClient StorageClient { get; }
    
    public StorageServiceFileStorageAdapter(StorageServiceClient storageClient)
    {
        StorageClient = storageClient;
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