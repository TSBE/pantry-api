using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;
using Pantry.Features.WebFeature.V1.Controllers.Requests;

namespace Pantry.Features.WebFeature.Commands;

public class UpdateArticleCommandHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<UpdateArticleCommandHandler> _logger;

    private readonly IPrincipal _principal;

    public UpdateArticleCommandHandler(
        ILogger<UpdateArticleCommandHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<Article> ExecuteAsync(UpdateArticleCommand command)
    {
        _logger.ExecutingCommand(nameof(UpdateArticleCommand));
        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var householdId = _principal.GetHouseholdIdOrThrow();
        var article = await appDbContext.Articles.Include(x => x.Household).FirstOrThrowAsync(c => c.Household.HouseholdId == householdId && c.ArticleId == command.ArticleId);
        var storageLocation = await appDbContext.StorageLocations.Include(i => i.Household).FirstOrThrowAsync(c => c.HouseholdId == householdId && c.StorageLocationId == command.StorageLocationId);

        article.StorageLocation = storageLocation;
        article.GlobalTradeItemNumber = command.GlobalTradeItemNumber;
        article.Name = command.Name;
        article.BestBeforeDate = command.BestBeforeDate;
        article.Quantity = command.Quantity;
        article.Content = command.Content;
        article.ContentType = command.ContentType;

        await appDbContext.SaveChangesAsync();

        return article;
    }
}
