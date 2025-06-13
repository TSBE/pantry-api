using System;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Features.WebFeature.V1.Controllers.Extensions;

public static class AccountResponseExtensions
{
    public static AccountResponse ToDtoNotNull(this Account model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new AccountResponse
        {
            FriendsCode = model.FriendsCode,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Household = model.Household?.ToDto()
        };
    }
}
