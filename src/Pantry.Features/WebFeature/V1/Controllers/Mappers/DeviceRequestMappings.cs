using Pantry.Core.Mappers;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Requests;

namespace Pantry.Features.WebFeature.V1.Controllers.Mappers
{
    public static class DeviceRequestMappings
    {
        private static readonly DtoModelMapping<DeviceRequest, Device> Mapping =
            new(
                config =>
                {
                    config.ForMember(o => o.DeviceToken, dest => dest.MapFrom(o => o.DeviceToken));
                    config.ForMember(o => o.InstallationId, dest => dest.MapFrom(o => o.InstallationId));
                    config.ForMember(o => o.Model, dest => dest.MapFrom(o => o.Model));
                    config.ForMember(o => o.Name, dest => dest.MapFrom(o => o.Name));
                    config.ForMember(o => o.Platform, dest => dest.MapFrom(o => o.Platform));
                },
                null);

        public static Device ToModelNotNull(this DeviceRequest dto)
        {
            return Mapping.ToModelNotNull(dto);
        }
    }
}
