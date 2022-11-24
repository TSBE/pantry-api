using System.Collections.Generic;
using Pantry.Core.Mappers;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Features.WebFeature.V1.Controllers.Mappers
{
    public static class StorageLocationResponseMappings
    {
        private static readonly DtoModelMapping<StorageLocationResponse, StorageLocation> Mapping =
            new(
                null,
                config =>
                {
                    config.ForMember(o => o.Id, dest => dest.MapFrom(o => o.StorageLocationId));
                    config.ForMember(o => o.Name, dest => dest.MapFrom(o => o.Name));
                    config.ForMember(o => o.Description, dest => dest.MapFrom(o => o.Description));
                });

        public static StorageLocationResponse? ToDto(this StorageLocation model)
        {
            return Mapping.ToDto(model);
        }

        public static StorageLocationResponse ToDtoNotNull(this StorageLocation model)
        {
            return Mapping.ToDtoNotNull(model);
        }

        public static StorageLocationResponse[] ToDtos(this IEnumerable<StorageLocation> models)
        {
            return Mapping.ToDtos(models);
        }
    }
}
