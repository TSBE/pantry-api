using System.Collections.Generic;
using Pantry.Core.Persistence.Entities;
using Silverback.Messaging.Messages;

namespace Pantry.Features.WebFeature.Queries;

public record InvitationListQuery() : IQuery<IReadOnlyCollection<Invitation>>;
