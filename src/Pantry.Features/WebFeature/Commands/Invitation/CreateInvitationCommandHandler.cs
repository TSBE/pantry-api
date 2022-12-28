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

public class CreateInvitationCommandHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<CreateInvitationCommandHandler> _logger;

    private readonly IPrincipal _principal;

    public CreateInvitationCommandHandler(
        ILogger<CreateInvitationCommandHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<Invitation> ExecuteAsync(CreateInvitationCommand command)
    {
        _logger.ExecutingCommand(nameof(CreateInvitationCommand));
        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();
        var auth0Id = _principal.GetAuth0IdOrThrow();
        var householdId = _principal.GetHouseholdIdOrThrow();

        var account = await appDbContext.Accounts.Include(x => x.Household).FirstOrThrowAsync(c => c.OAuhtId == auth0Id);

        if (account.Household is null || !account.AccountId.Equals(account.Household.OwnerId))
        {
            throw new ForbiddenException();
        }

        var invitation = await appDbContext.Invitations.FirstOrDefaultAsync(c => c.CreatorId == account.AccountId && c.FriendsCode == command.FriendsCode && c.HouseholdId == householdId);

        if (invitation is null)
        {
            invitation = new Invitation
            {
                ValidUntilDate = DateTimeProvider.UtcNow.AddDays(10),
                CreatorId = account.AccountId,
                Household = account.Household,
                FriendsCode = command.FriendsCode
            };

            appDbContext.Invitations.Add(invitation);
            await appDbContext.SaveChangesAsync();
        }
        else
        {
            throw new BadRequestException();
        }

        return invitation;
    }
}
