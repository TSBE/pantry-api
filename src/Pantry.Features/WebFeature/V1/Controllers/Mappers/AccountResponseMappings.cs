using System.Collections.Generic;
using Pantry.Core.Mappers;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Features.WebFeature.V1.Controllers.Mappers
{
    public static class AccountResponseMappings
    {
        private static readonly DtoModelMapping<AccountResponse, Account> Mapping =
            new(
                null,
                config =>
                {
                    config.ForMember(o => o.FriendsCode, dest => dest.MapFrom(o => o.FriendsCode));
                    config.ForMember(o => o.FirstName, dest => dest.MapFrom(o => o.FirstName));
                    config.ForMember(o => o.LastName, dest => dest.MapFrom(o => o.LastName));
                    config.ForMember(o => o.Household, dest => dest.MapFrom(o => o.Household.ToDto()));

                });

        public static AccountResponse? ToDto(this Account model)
        {
            return Mapping.ToDto(model);
        }

        public static AccountResponse ToDtoNotNull(this Account model)
        {
            return Mapping.ToDtoNotNull(model);
        }

        public static AccountResponse[] ToDtos(this IEnumerable<Account> models)
        {
            return Mapping.ToDtos(models);
        }
    }
}
