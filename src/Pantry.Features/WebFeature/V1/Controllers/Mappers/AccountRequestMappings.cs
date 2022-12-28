using Pantry.Core.Mappers;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Requests;

namespace Pantry.Features.WebFeature.V1.Controllers.Mappers
{
    public static class AccountRequestMappings
    {
        private static readonly DtoModelMapping<AccountRequest, Account> Mapping =
            new(
                config =>
                {
                    config.ForMember(o => o.FirstName, dest => dest.MapFrom(o => o.FirstName));
                    config.ForMember(o => o.LastName, dest => dest.MapFrom(o => o.LastName));
                },
                null);

        public static Account ToModelNotNull(this AccountRequest dto)
        {
            return Mapping.ToModelNotNull(dto);
        }
    }
}
