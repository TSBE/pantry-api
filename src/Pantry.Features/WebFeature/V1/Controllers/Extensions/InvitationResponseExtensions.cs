using System;
using System.Collections.Generic;
using System.Linq;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Features.WebFeature.V1.Controllers.Extensions;

public static class InvitationResponseExtensions
{
    public static InvitationResponse ToDtoNotNull(this Invitation model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new InvitationResponse
        {
            CreatorName = $"{model.Creator.FirstName} {model.Creator.LastName}",
            FriendsCode = model.FriendsCode,
            HouseholdName = model.Household.Name,
            ValidUntilDate = model.ValidUntilDate
        };
    }

    public static InvitationResponse? ToDto(this Invitation? model)
    {
        return model?.ToDtoNotNull();
    }

    public static InvitationResponse[] ToDtos(this IEnumerable<Invitation> models)
    {
        ArgumentNullException.ThrowIfNull(models);
        return models.Select(m => m.ToDtoNotNull()).ToArray();
    }
}
