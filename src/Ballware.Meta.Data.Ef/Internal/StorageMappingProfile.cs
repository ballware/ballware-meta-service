using AutoMapper;

namespace Ballware.Meta.Data.Ef.Internal;

class StorageMappingProfile : Profile
{
    public StorageMappingProfile()
    {
        CreateMap<Public.Tenant, Persistables.Tenant>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Id));

        CreateMap<Persistables.Tenant, Public.Tenant>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid))
            .ForMember(dest => dest.DatabaseObjects, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    if (context.TryGetItems(out var items) && items.TryGetValue("DatabaseObjects", out var databaseObjects))
                    {
                        return databaseObjects as IEnumerable<Public.TenantDatabaseObject>;
                    }

                    return null;
                }
            ));

        CreateMap<Public.Documentation, Persistables.Documentation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Id));

        CreateMap<Persistables.Documentation, Public.Documentation>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid));

        CreateMap<Public.Document, Persistables.Document>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Id));

        CreateMap<Persistables.Document, Public.Document>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid));

        CreateMap<Public.EntityMetadata, Persistables.EntityMetadata>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Id));

        CreateMap<Persistables.EntityMetadata, Public.EntityMetadata>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid));

        CreateMap<Public.Export, Persistables.Export>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Id));

        CreateMap<Persistables.Export, Public.Export>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid));

        CreateMap<Public.Job, Persistables.Job>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Id));

        CreateMap<Persistables.Job, Public.Job>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid));

        CreateMap<Public.Lookup, Persistables.Lookup>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Id));

        CreateMap<Persistables.Lookup, Public.Lookup>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid));

        CreateMap<Public.MlModel, Persistables.MlModel>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Id));

        CreateMap<Persistables.MlModel, Public.MlModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid));

        CreateMap<Public.Notification, Persistables.Notification>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Id));

        CreateMap<Persistables.Notification, Public.Notification>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid));

        CreateMap<Public.NotificationTrigger, Persistables.NotificationTrigger>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Id));

        CreateMap<Persistables.NotificationTrigger, Public.NotificationTrigger>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid));

        CreateMap<Public.Page, Persistables.Page>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Id));

        CreateMap<Persistables.Page, Public.Page>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid));

        CreateMap<Public.Pickvalue, Persistables.Pickvalue>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Id));

        CreateMap<Persistables.Pickvalue, Public.Pickvalue>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid));

        CreateMap<Public.ProcessingState, Persistables.ProcessingState>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Id));

        CreateMap<Persistables.ProcessingState, Public.ProcessingState>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid));

        CreateMap<Public.Statistic, Persistables.Statistic>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Id));

        CreateMap<Persistables.Statistic, Public.Statistic>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid));

        CreateMap<Public.Subscription, Persistables.Subscription>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Id));

        CreateMap<Persistables.Subscription, Public.Subscription>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid));
        
        CreateMap<Public.TenantDatabaseObject, Persistables.TenantDatabaseObject>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Id));

        CreateMap<Persistables.TenantDatabaseObject, Public.TenantDatabaseObject>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Uuid));
    }
}