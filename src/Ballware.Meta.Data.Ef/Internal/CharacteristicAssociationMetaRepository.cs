using AutoMapper;
using Ballware.Meta.Data.Persistables;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class CharacteristicAssociationMetaRepository : TenantableBaseRepository<Public.CharacteristicAssociation, Persistables.CharacteristicAssociation>, ICharacteristicAssociationMetaRepository
{
    public CharacteristicAssociationMetaRepository(IMapper mapper, MetaDbContext dbContext, ITenantableRepositoryHook<Public.CharacteristicAssociation, Persistables.CharacteristicAssociation>? hook = null) 
        : base(mapper, dbContext, hook) { }

    protected override IQueryable<CharacteristicAssociation> ListQuery(IQueryable<CharacteristicAssociation> query, string identifier, IDictionary<string, object> claims, IDictionary<string, object> queryParams)
    {
        if ("entity".Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
        {
            if (!queryParams.TryGetValue("entity", out var entity)) 
            {
                throw new ArgumentException("Entity parameter is required");
            }

            return query.Where(er => er.Entity == entity.ToString());
        }
        
        return base.ListQuery(query, identifier, claims, queryParams);
    }
}