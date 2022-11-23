using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Commands;

public class CreateAccountCommandHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<CreateAccountCommandHandler> _logger;

    private readonly IPrincipal _principal;

    public CreateAccountCommandHandler(
        ILogger<CreateAccountCommandHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<Account> ExecuteAsync(CreateAccountCommand command)
    {
        _logger.ExecutingCommand(nameof(CreateAccountCommand));
        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var auth0Id = _principal.GetAuth0IdOrThrow();

        var account = await appDbContext.Accounts.FirstOrDefaultAsync(c => c.OAuhtId == auth0Id);

        if (account is null)
        {
            account = new Account
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                FriendsCode = Guid.NewGuid(),
                OAuhtId = auth0Id
            };
            appDbContext.Accounts.Add(account);
            await appDbContext.SaveChangesAsync();
        }
        else
        {
            account.FirstName = command.FirstName;
            account.LastName = command.LastName;
            await appDbContext.SaveChangesAsync();
        }

        return account;
    }
}
