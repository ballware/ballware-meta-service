namespace Ballware.Meta.Service.Dtos;

public class ServiceExportDto
{
    public Guid Id { get; set; }

    public string? Application { get; set; }
    public string? Entity { get; set; }
    public string? Query { get; set; }
    public DateTime? ExpirationStamp { get; set; }
    public string? MediaType { get; set; }
}