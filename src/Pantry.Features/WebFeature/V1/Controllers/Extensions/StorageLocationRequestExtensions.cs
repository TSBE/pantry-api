using System;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Requests;

namespace Pantry.Features.WebFeature.V1.Controllers.Extensions;

public static class StorageLocationRequestExtensions
{
    public static StorageLocation ToModelNotNull(this StorageLocationRequest dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new StorageLocation
        {
            Name = dto.Name,
            Description = dto.Description
        };
    }
}
