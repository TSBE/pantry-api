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

public class CreateHouseholdCommandHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<CreateHouseholdCommandHandler> _logger;

    private readonly IPrincipal _principal;

    public CreateHouseholdCommandHandler(
        ILogger<CreateHouseholdCommandHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<Household> ExecuteAsync(CreateHouseholdCommand command)
    {
        _logger.ExecutingCommand(nameof(CreateHouseholdCommand));
        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();
        var auth0Id = _principal.GetAuth0IdOrThrow();
        var account = await appDbContext.Accounts.Include(x => x.Household).FirstOrThrowAsync(c => c.OAuhtId == auth0Id);
        var household = account.Household;

        if (household is null)
        {
            household = new Household
            {
                Name = command.Name,
                SubscriptionType = command.SubscriptionType,
            };
            account.Household = household;
            appDbContext.Households.Add(household);
            await appDbContext.SaveChangesAsync();
        }
        else
        {
            throw new BadRequestException();
        }

        return household;
    }
}
