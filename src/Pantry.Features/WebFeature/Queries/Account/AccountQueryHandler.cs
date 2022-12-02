using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Queries;

public class AccountQueryHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<AccountQueryHandler> _logger;

    private readonly IPrincipal _principal;

    public AccountQueryHandler(
        ILogger<AccountQueryHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<Account> ExecuteAsync(AccountQuery query)
    {
        _logger.ExecutingQuery(nameof(AccountQuery));

        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();
        var auth0Id = _principal.GetAuth0IdOrThrow();
        return await appDbContext.Accounts.Include(i => i.Household).AsNoTracking().FirstOrThrowAsync(c => c.OAuhtId == auth0Id);
    }
}
