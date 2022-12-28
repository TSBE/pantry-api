using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Queries;

public class ArticleByIdQueryHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<ArticleByIdQueryHandler> _logger;

    private readonly IPrincipal _principal;

    public ArticleByIdQueryHandler(
        ILogger<ArticleByIdQueryHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<Article> ExecuteAsync(ArticleByIdQuery query)
    {
        _logger.ExecutingQuery(nameof(ArticleByIdQuery));

        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var householdId = _principal.GetHouseholdIdOrThrow();
        var article = await appDbContext.Articles.Include(i => i.StorageLocation).Include(x => x.Household).AsNoTracking().FirstOrThrowAsync(c => c.Household.HouseholdId == householdId && c.ArticleId == query.ArticleId);

        if (article.GlobalTradeItemNumber is not null)
        {
            article.Metadata = await appDbContext.Metadatas.AsNoTracking().FirstOrDefaultAsync(x => x.GlobalTradeItemNumber == article.GlobalTradeItemNumber);
        }

        return article;
    }
}
