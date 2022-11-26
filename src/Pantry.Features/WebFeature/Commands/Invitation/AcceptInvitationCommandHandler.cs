using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Opw.HttpExceptions;
using Pantry.Common.Authentication;
using Pantry.Common.Time;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Commands;

public class AcceptInvitationCommandHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<AcceptInvitationCommandHandler> _logger;

    private readonly IPrincipal _principal;

    public AcceptInvitationCommandHandler(
        ILogger<AcceptInvitationCommandHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task ExecuteAsync(AcceptInvitationCommand command)
    {
        _logger.ExecutingCommand(nameof(AcceptInvitationCommand));
        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();
        var auth0Id = _principal.GetAuth0IdOrThrow();
        var account = await appDbContext.Accounts.Include(x => x.Household).FirstOrThrowAsync(c => c.OAuhtId == auth0Id);

        if (account.Household is not null || !account.FriendsCode.Equals(command.FriendsCode))
        {
            throw new ForbiddenException();
        }

        Invitation invitation = await appDbContext.Invitations.Include(x => x.Household).FirstOrThrowAsync(c => c.FriendsCode == command.FriendsCode);

        if (invitation.ValidUntilDate <= DateTimeProvider.UtcNow)
        {
            appDbContext.Invitations.Remove(invitation);
            await appDbContext.SaveChangesAsync();

            throw new BadRequestException();
        }
        else
        {
            account.Household = invitation.Household;
            appDbContext.Invitations.Remove(invitation);
            await appDbContext.SaveChangesAsync();
        }
    }
}
