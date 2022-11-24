using System;
using Pantry.Core.Persistence.Entities;
using Pantry.Core.Persistence.Enums;
using Silverback.Messaging.Messages;

namespace Pantry.Features.WebFeature.Commands;

public record UpdateArticleCommand(long ArticleId, long StorageLocationId, string? GlobalTradeItemNumber, string Name, DateTime BestBeforeDate, int Quantity, string? Content, ContentType ContentType) : ICommand<Article>;
