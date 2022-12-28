using System;
using Pantry.Core.Persistence.Entities;
using Silverback.Messaging.Messages;

namespace Pantry.Features.WebFeature.Commands;

public record UpdateDeviceCommand(Guid InstallationId, string Name, string? DeviceToken) : ICommand<Device>;
