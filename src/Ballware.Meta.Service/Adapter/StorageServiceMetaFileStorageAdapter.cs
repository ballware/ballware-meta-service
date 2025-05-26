using Ballware.Meta.Api;
using Ballware.Storage.Client;

namespace Ballware.Meta.Service.Adapter;

public class StorageServiceMetaFileStorageAdapter : IMetaFileStorageAdapter
{
    private BallwareStorageClient StorageClient { get; }
    
    public StorageServiceMetaFileStorageAdapter(BallwareStorageClient storageClient)
    {
        StorageClient = storageClient;
    }
    
    public async Task<Stream> FileByNameForOwnerAsync(string owner, string fileName)
    {
        var result = await StorageClient.FileByNameForOwnerAsync(owner, fileName);
        
        return result.Stream;
    }

    public async Task UploadFileForOwnerAsync(string owner, string fileName, string contentType, Stream data)
    {
        await StorageClient.UploadFileForOwnerAsync(owner, new []{ new FileParameter(data, fileName, contentType) });
    }
}