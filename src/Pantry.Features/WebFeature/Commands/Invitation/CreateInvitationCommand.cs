using System;
using Pantry.Core.Persistence.Entities;
using Silverback.Messaging.Messages;

namespace Pantry.Features.WebFeature.Commands;

public record CreateInvitationCommand(Guid FriendsCode) : ICommand<Invitation>;
