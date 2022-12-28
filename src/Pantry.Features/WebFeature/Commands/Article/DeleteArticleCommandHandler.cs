using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Commands;

public class DeleteArticleCommandHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<DeleteArticleCommandHandler> _logger;

    private readonly IPrincipal _principal;

    public DeleteArticleCommandHandler(
        ILogger<DeleteArticleCommandHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task ExecuteAsync(DeleteArticleCommand command)
    {
        _logger.ExecutingCommand(nameof(DeleteArticleCommand));
        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var householdId = _principal.GetHouseholdIdOrThrow();
        var article = await appDbContext.Articles.Include(x => x.Household).FirstOrThrowAsync(c => c.Household.HouseholdId == householdId && c.ArticleId == command.ArticleId);

        appDbContext.Articles.Remove(article);
        await appDbContext.SaveChangesAsync();
    }
}
