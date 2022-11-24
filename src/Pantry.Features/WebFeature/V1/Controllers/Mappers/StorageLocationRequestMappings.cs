using Pantry.Core.Mappers;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Requests;

namespace Pantry.Features.WebFeature.V1.Controllers.Mappers
{
    public static class StorageLocationRequestMappings
    {
        private static readonly DtoModelMapping<StorageLocationRequest, StorageLocation> Mapping =
            new(
                config =>
                {
                    config.ForMember(o => o.Name, dest => dest.MapFrom(o => o.Name));
                    config.ForMember(o => o.Description, dest => dest.MapFrom(o => o.Description));
                },
                null);

        public static StorageLocation ToModelNotNull(this StorageLocationRequest dto)
        {
            return Mapping.ToModelNotNull(dto);
        }
    }
}
