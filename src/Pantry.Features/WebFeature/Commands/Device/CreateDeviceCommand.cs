using System;
using Pantry.Core.Persistence.Entities;
using Pantry.Core.Persistence.Enums;
using Silverback.Messaging.Messages;

namespace Pantry.Features.WebFeature.Commands;

public record CreateDeviceCommand(string? DeviceToken, Guid InstallationId, string Model, string Name, DevicePlatformType Platform) : ICommand<Device>;
