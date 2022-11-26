using System.Collections.Generic;
using Pantry.Core.Mappers;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Features.WebFeature.V1.Controllers.Mappers
{
    public static class InvitationResponseMappings
    {
        private static readonly DtoModelMapping<InvitationResponse, Invitation> Mapping =
            new(
                null,
                config =>
                {
                    config.ForMember(o => o.CreatorName, dest => dest.MapFrom(o => $"{o.Creator.FirstName} {o.Creator.LastName}"));
                    config.ForMember(o => o.FriendsCode, dest => dest.MapFrom(o => o.FriendsCode));
                    config.ForMember(o => o.HouseholdName, dest => dest.MapFrom(o => o.Household.Name));
                    config.ForMember(o => o.ValidUntilDate, dest => dest.MapFrom(o => o.ValidUntilDate));
                });

        public static InvitationResponse? ToDto(this Invitation model)
        {
            return Mapping.ToDto(model);
        }

        public static InvitationResponse ToDtoNotNull(this Invitation model)
        {
            return Mapping.ToDtoNotNull(model);
        }

        public static InvitationResponse[] ToDtos(this IEnumerable<Invitation> models)
        {
            return Mapping.ToDtos(models);
        }
    }
}
