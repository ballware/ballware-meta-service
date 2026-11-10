using Mapster;

namespace Ballware.Meta.Data.Ef.Mapping;

class StorageMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Public.Tenant, Persistables.Tenant>()
            .Ignore(dest => dest.Id!)
            .Map(dest => dest.Uuid, src => src.Id);
        
        config.NewConfig<Persistables.Tenant, Public.Tenant>()
            .Map(dest => dest.Id, src => src.Uuid);
        
        config.NewConfig<Public.Documentation, Persistables.Documentation>()
            .Ignore(dest => dest.Id!)
            .Map(dest => dest.Uuid, src => src.Id);
        
        config.NewConfig<Persistables.Documentation, Public.Documentation>()
            .Map(dest => dest.Id, src => src.Uuid);

        config.NewConfig<Public.EntityMetadata, Persistables.EntityMetadata>()
            .Ignore(dest => dest.Id!)
            .Map(dest => dest.Uuid, src => src.Id);
        
        config.NewConfig<Persistables.EntityMetadata, Public.EntityMetadata>()
            .Map(dest => dest.Id, src => src.Uuid);

        config.NewConfig<Public.Export, Persistables.Export>()
            .Ignore(dest => dest.Id!)
            .Map(dest => dest.Uuid, src => src.Id);
        
        config.NewConfig<Persistables.Export, Public.Export>()
            .Map(dest => dest.Id, src => src.Uuid);

        config.NewConfig<Public.Job, Persistables.Job>()
            .Ignore(dest => dest.Id!)
            .Map(dest => dest.Uuid, src => src.Id);
        
        config.NewConfig<Persistables.Job, Public.Job>()
            .Map(dest => dest.Id, src => src.Uuid);

        config.NewConfig<Public.Lookup, Persistables.Lookup>()
            .Ignore(dest => dest.Id!)
            .Map(dest => dest.Uuid, src => src.Id);
        
        config.NewConfig<Persistables.Lookup, Public.Lookup>()
            .Map(dest => dest.Id, src => src.Uuid);

        config.NewConfig<Public.Page, Persistables.Page>()
            .Ignore(dest => dest.Id!)
            .Map(dest => dest.Uuid, src => src.Id);
        
        config.NewConfig<Persistables.Page, Public.Page>()
            .Map(dest => dest.Id, src => src.Uuid);

        config.NewConfig<Public.Pickvalue, Persistables.Pickvalue>()
            .Ignore(dest => dest.Id!)
            .Map(dest => dest.Uuid, src => src.Id);
        
        config.NewConfig<Persistables.Pickvalue, Public.Pickvalue>()
            .Map(dest => dest.Id, src => src.Uuid);

        config.NewConfig<Public.ProcessingState, Persistables.ProcessingState>()
            .Ignore(dest => dest.Id!)
            .Map(dest => dest.Uuid, src => src.Id);
        
        config.NewConfig<Persistables.ProcessingState, Public.ProcessingState>()
            .Map(dest => dest.Id, src => src.Uuid);

        config.NewConfig<Public.EntityRight, Persistables.EntityRight>()
            .Ignore(dest => dest.Id!)
            .Map(dest => dest.Uuid, src => src.Id);
        
        config.NewConfig<Persistables.EntityRight, Public.EntityRight>()
            .Map(dest => dest.Id, src => src.Uuid);

        config.NewConfig<Public.Statistic, Persistables.Statistic>()
            .Ignore(dest => dest.Id!)
            .Map(dest => dest.Uuid, src => src.Id);
        
        config.NewConfig<Persistables.Statistic, Public.Statistic>()
            .Map(dest => dest.Id, src => src.Uuid);
    }
}