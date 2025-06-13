using System;
using System.Collections.Generic;
using System.Linq;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Features.WebFeature.V1.Controllers.Extensions;

public static class StorageLocationResponseExtensions
{
    public static StorageLocationResponse? ToDto(this StorageLocation model)
    {
        return model.ToDtoNotNull();
    }

    public static StorageLocationResponse ToDtoNotNull(this StorageLocation model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new StorageLocationResponse
        {
            Id = model.StorageLocationId,
            Name = model.Name,
            Description = model?.Description ?? string.Empty
        };
    }

    public static StorageLocationResponse[] ToDtos(this IEnumerable<StorageLocation> models)
    {
        ArgumentNullException.ThrowIfNull(models);

        return models
            .Where(m => m != null)
            .Select(m => m.ToDtoNotNull())
            .ToArray();
    }
}
