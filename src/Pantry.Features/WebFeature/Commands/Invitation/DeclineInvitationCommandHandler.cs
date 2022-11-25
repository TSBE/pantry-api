using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opw.HttpExceptions;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Commands;

public class DeclineInvitationCommandHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<DeclineInvitationCommandHandler> _logger;

    private readonly IPrincipal _principal;

    public DeclineInvitationCommandHandler(
        ILogger<DeclineInvitationCommandHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task ExecuteAsync(DeclineInvitationCommand command)
    {
        _logger.ExecutingCommand(nameof(DeclineInvitationCommand));
        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();
        var auth0Id = _principal.GetAuth0IdOrThrow();
        var account = await appDbContext.Accounts.FirstOrThrowAsync(c => c.OAuhtId == auth0Id);

        if (!account.FriendsCode.Equals(command.FriendsCode))
        {
            throw new ForbiddenException();
        }

        Invitation invitation = await appDbContext.Invitations.FirstOrThrowAsync(c => c.FriendsCode == command.FriendsCode);
        appDbContext.Invitations.Remove(invitation);
        await appDbContext.SaveChangesAsync();
    }
}
