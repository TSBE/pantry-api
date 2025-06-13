using System;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Requests;

namespace Pantry.Features.WebFeature.V1.Controllers.Extensions;

public static class DeviceRequestExtensions
{
    public static Device ToModelNotNull(this DeviceRequest dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new Device
        {
            DeviceToken = dto.DeviceToken,
            InstallationId = dto.InstallationId,
            Model = dto.Model,
            Name = dto.Name,
            Platform = dto.Platform.ToModelNotNull()
        };
    }
}
