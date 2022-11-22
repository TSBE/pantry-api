#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using System;
using Pantry.Core.Persistence.Enums;

namespace Pantry.Core.Persistence.Entities;

/// <summary>
/// Represents a users device.
/// </summary>
public class Device : Auditable
{
    /// <summary>
    /// Represents the database internal id.
    /// </summary>
    public long DeviceId { get; set; }

    /// <summary>
    /// A device name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The device model.
    /// </summary>
    public string Model { get; set; }

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

    public long AccountId { get; set; }

    public virtual Account Account { get; set; }
}
