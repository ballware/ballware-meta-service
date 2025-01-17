using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Ballware.Meta.Data.Public;

public class Notification : IEditable
{
    public Guid Id { get; set; }
    
    public string? Identifier { get; set; }
    
    public string? Name { get; set; }
    public Guid? DocumentId { get; set; }
    public int State { get; set; }
    public string? Params { get; set; }
}