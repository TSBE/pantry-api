using System;
using Silverback.Messaging.Messages;

namespace Pantry.Features.WebFeature.Commands;

public record DeleteDeviceCommand(Guid InstallationId) : ICommand;
