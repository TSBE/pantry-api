using System;
using System.Collections.Generic;
using System.Linq;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Features.WebFeature.V1.Controllers.Extensions;

public static class HouseholdResponseExtensions
{
    public static HouseholdResponse ToDtoNotNull(this Household model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new HouseholdResponse
        {
            Name = model.Name,
            SubscriptionType = model.SubscriptionType.ToDtoNotNull()
        };
    }

    public static HouseholdResponse? ToDto(this Household? model)
    {
        return model?.ToDtoNotNull();
    }

    public static HouseholdResponse[] ToDtos(this IEnumerable<Household> models)
    {
        ArgumentNullException.ThrowIfNull(models);

        return models.Select(m => m.ToDtoNotNull()).ToArray();
    }
}
