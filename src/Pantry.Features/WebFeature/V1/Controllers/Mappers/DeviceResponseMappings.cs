using System.Collections.Generic;
using Pantry.Core.Mappers;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Features.WebFeature.V1.Controllers.Mappers
{
    public static class DeviceResponseMappings
    {
        private static readonly DtoModelMapping<DeviceResponse, Device> Mapping =
            new(
                null,
                config =>
                {
                    config.ForMember(o => o.DeviceToken, dest => dest.MapFrom(o => o.DeviceToken));
                    config.ForMember(o => o.InstallationId, dest => dest.MapFrom(o => o.InstallationId));
                    config.ForMember(o => o.Model, dest => dest.MapFrom(o => o.Model));
                    config.ForMember(o => o.Name, dest => dest.MapFrom(o => o.Name));
                    config.ForMember(o => o.Platform, dest => dest.MapFrom(o => o.Platform));
                });

        public static DeviceResponse? ToDto(this Device model)
        {
            return Mapping.ToDto(model);
        }

        public static DeviceResponse ToDtoNotNull(this Device model)
        {
            return Mapping.ToDtoNotNull(model);
        }

        public static DeviceResponse[] ToDtos(this IEnumerable<Device> models)
        {
            return Mapping.ToDtos(models);
        }
    }
}
