using Ballware.Meta.Data.Common;
using Ballware.Shared.Data.Public;

namespace Ballware.Meta.Data.Public;

public class CharacteristicAssociation : IEditable
{
    public Guid Id { get; set; }

    public string? Entity { get; set; }
    public string? Identifier { get; set; }
    public EntityCharacteristicTypes Type { get; set; }
    public int? Length { get; set; }
    public Guid? CharacteristicId { get; set; }
    public Guid? CharacteristicGroupId { get; set; }
    public bool? Active { get; set; }
    public bool? Required { get; set; }
    public bool? Readonly { get; set; }
    public int? Sorting { get; set; }
}
