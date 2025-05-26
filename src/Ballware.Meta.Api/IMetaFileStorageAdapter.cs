namespace Ballware.Meta.Api;

public interface IMetaFileStorageAdapter
{
    Task<Stream> FileByNameForOwnerAsync(string owner, string fileName);
    Task UploadFileForOwnerAsync(string owner, string fileName, string contentType, Stream data);
}