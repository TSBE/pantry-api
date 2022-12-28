using System.Collections.Generic;
using Pantry.Core.Mappers;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Features.WebFeature.V1.Controllers.Mappers
{
    public static class HouseholdResponseMappings
    {
        private static readonly DtoModelMapping<HouseholdResponse, Household> Mapping =
            new(
                null,
                config =>
                {
                    config.ForMember(o => o.Name, dest => dest.MapFrom(o => o.Name));
                    config.ForMember(o => o.SubscriptionType, dest => dest.MapFrom(o => o.SubscriptionType));
                });

        public static HouseholdResponse? ToDto(this Household model)
        {
            return Mapping.ToDto(model);
        }

        public static HouseholdResponse ToDtoNotNull(this Household model)
        {
            return Mapping.ToDtoNotNull(model);
        }

        public static HouseholdResponse[] ToDtos(this IEnumerable<Household> models)
        {
            return Mapping.ToDtos(models);
        }
    }
}
