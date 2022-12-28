using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Queries;

public class ArticleListQueryHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<ArticleListQueryHandler> _logger;

    private readonly IPrincipal _principal;

    public ArticleListQueryHandler(
        ILogger<ArticleListQueryHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<IReadOnlyCollection<Article>> ExecuteAsync(ArticleListQuery query)
    {
        _logger.ExecutingQuery(nameof(ArticleListQuery));

        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var householdId = _principal.GetHouseholdIdOrThrow();
        return await appDbContext.Articles.Include(i => i.StorageLocation).Include(x => x.Household).AsNoTracking().Where(c => c.Household.HouseholdId == householdId).ToListAsync();
    }
}
