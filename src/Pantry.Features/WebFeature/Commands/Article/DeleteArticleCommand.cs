using Pantry.Core.Persistence.Entities;
using Silverback.Messaging.Messages;

namespace Pantry.Features.WebFeature.Commands;

public record DeleteArticleCommand(long ArticleId) : ICommand<Article>;
