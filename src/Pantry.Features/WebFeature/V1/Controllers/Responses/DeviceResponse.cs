using System;
using Pantry.Features.WebFeature.V1.Controllers.Enums;

namespace Pantry.Features.WebFeature.V1.Controllers.Responses;

public class DeviceResponse
{
    /// <summary>
    /// A device name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The device model.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// The device token for push notifications.
    /// </summary>
    public string? DeviceToken { get; set; }

    /// <summary>
    /// Information about the operating system.
    /// </summary>
    public DevicePlatformType Platform { get; set; }

    /// <summary>
    /// Installation identifier.
    /// </summary>
    public Guid InstallationId { get; set; }
}
