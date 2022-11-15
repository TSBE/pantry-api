using Pantry.Core.Persistence.Entities;
using Silverback.Messaging.Messages;

namespace Pantry.Features.WebFeature.Queries;

public record DeviceByIdQuery(long DeviceId) : IQuery<Device>;
