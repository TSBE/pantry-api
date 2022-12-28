using Pantry.Core.Persistence.Entities;
using Silverback.Messaging.Messages;

namespace Pantry.Features.WebFeature.Commands;

public record DeleteStorageLocationCommand(long StorageLocationId) : ICommand<StorageLocation>;
