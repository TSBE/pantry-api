using Pantry.Core.Persistence.Entities;
using Silverback.Messaging.Messages;

namespace Pantry.Features.WebFeature.Commands;

public record CreateAccountCommand(string FirstName, string LastName) : ICommand<Account>;
