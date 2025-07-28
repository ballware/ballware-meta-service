namespace Ballware.Meta.Api;

public interface IMetaFileStorageAdapter
{
    Task<Stream> TemporaryFileByIdAsync(Guid tenantId, Guid temporaryId);
    Task UploadTemporaryFileBehalfOfUserAsync(Guid tenantId, Guid userId, Guid temporaryId, string fileName, string contentType, Stream data);
}