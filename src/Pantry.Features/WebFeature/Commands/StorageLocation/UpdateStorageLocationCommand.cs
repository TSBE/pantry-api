using Pantry.Core.Persistence.Entities;
using Silverback.Messaging.Messages;

namespace Pantry.Features.WebFeature.Commands;

public record UpdateStorageLocationCommand(long StorageLocationId, string Name, string? Description) : ICommand<StorageLocation>;
