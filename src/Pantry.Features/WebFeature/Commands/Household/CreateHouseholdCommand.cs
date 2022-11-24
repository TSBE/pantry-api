using Pantry.Core.Persistence.Entities;
using Pantry.Core.Persistence.Enums;
using Silverback.Messaging.Messages;

namespace Pantry.Features.WebFeature.Commands;

public record CreateHouseholdCommand(string Name, SubscriptionType SubscriptionType) : ICommand<Household>;
