using System;
using System.Collections.Generic;
using System.Linq;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Features.WebFeature.V1.Controllers.Extensions;

public static class DeviceResponseExtensions
{
    public static DeviceResponse ToDtoNotNull(this Device model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new DeviceResponse
        {
            DeviceToken = model.DeviceToken,
            InstallationId = model.InstallationId,
            Model = model.Model,
            Name = model.Name,
            Platform = model.Platform.ToDtoNotNull()
        };
    }

    public static DeviceResponse? ToDto(this Device? model)
    {
        return model?.ToDtoNotNull();
    }

    public static DeviceResponse[] ToDtos(this IEnumerable<Device> models)
    {
        ArgumentNullException.ThrowIfNull(models);
        return models.Select(m => m.ToDtoNotNull()).ToArray();
    }
}
