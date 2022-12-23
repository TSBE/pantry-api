using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Commands;

public class CreateArticleCommandHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<CreateArticleCommandHandler> _logger;

    private readonly IPrincipal _principal;

    public CreateArticleCommandHandler(
        ILogger<CreateArticleCommandHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<Article> ExecuteAsync(CreateArticleCommand command)
    {
        _logger.ExecutingCommand(nameof(CreateArticleCommand));
        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var householdId = _principal.GetHouseholdIdOrThrow();
        var household = await appDbContext.Households.FirstOrThrowAsync(c => c.HouseholdId == householdId);
        var storageLocation = await appDbContext.StorageLocations.Include(i => i.Household).FirstOrThrowAsync(c => c.HouseholdId == householdId && c.StorageLocationId == command.StorageLocationId);
        var metadata = await appDbContext.Metadatas.FirstOrDefaultAsync(x => x.GlobalTradeItemNumber == command.GlobalTradeItemNumber);

        var article = new Article
        {
            Household = household,
            StorageLocation = storageLocation,
            GlobalTradeItemNumber = command.GlobalTradeItemNumber,
            Name = command.Name,
            BestBeforeDate = command.BestBeforeDate,
            Quantity = command.Quantity,
            Content = command.Content,
            ContentType = command.ContentType,
        };

        if (metadata is not null)
        {
            article.ImageUrl = metadata.FoodFacts?.ImageUrl ?? metadata.FoodFacts?.ImageFrontUrl ?? null;
        }

        appDbContext.Articles.Add(article);
        await appDbContext.SaveChangesAsync();

        return article;
    }
}
