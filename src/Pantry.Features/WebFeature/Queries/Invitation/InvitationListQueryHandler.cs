using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Queries;

public class InvitationListQueryHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<InvitationListQueryHandler> _logger;

    private readonly IPrincipal _principal;

    public InvitationListQueryHandler(
        ILogger<InvitationListQueryHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<IReadOnlyCollection<Invitation>> ExecuteAsync(InvitationListQuery query)
    {
        _logger.ExecutingQuery(nameof(InvitationListQuery));

        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var auth0Id = _principal.GetAuth0IdOrThrow();
        var account = await appDbContext.Accounts.AsNoTracking().FirstOrThrowAsync(c => c.OAuhtId == auth0Id);

        var invation = await appDbContext.Invitations.Include(i => i.Household).Include(i => i.Creator).AsNoTracking().FirstOrDefaultAsync(c => c.CreatorId == account.AccountId || c.FriendsCode == account.FriendsCode);
        return invation is not null ? new List<Invitation> { invation } : new List<Invitation>();
    }
}
