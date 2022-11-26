using System;
using Silverback.Messaging.Messages;

namespace Pantry.Features.WebFeature.Commands;

public record AcceptInvitationCommand(Guid FriendsCode) : ICommand;
