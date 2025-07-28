using AutoMapper;
using Ballware.Meta.Data.Persistables;
using Ballware.Meta.Data.Repository;
using Ballware.Meta.Data.SelectLists;
using Ballware.Shared.Data.Ef.Repository;
using Ballware.Shared.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace Ballware.Meta.Data.Ef.Internal;

class CharacteristicAssociationMetaRepository : TenantableRepository<Public.CharacteristicAssociation, Persistables.CharacteristicAssociation>, ICharacteristicAssociationMetaRepository
{
    public CharacteristicAssociationMetaRepository(IMapper mapper, IMetaDbContext dbContext, ITenantableRepositoryHook<Public.CharacteristicAssociation, Persistables.CharacteristicAssociation>? hook = null) 
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