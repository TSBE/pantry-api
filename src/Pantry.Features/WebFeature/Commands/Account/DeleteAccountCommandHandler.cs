using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Commands;

public class DeleteAccountCommandHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<DeleteAccountCommandHandler> _logger;

    private readonly IPrincipal _principal;

    public DeleteAccountCommandHandler(
        ILogger<DeleteAccountCommandHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task ExecuteAsync(DeleteAccountCommand command)
    {
        _logger.ExecutingCommand(nameof(DeleteAccountCommand));
        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var auth0Id = _principal.GetAuth0IdOrThrow();
        var account = await appDbContext.Accounts.FirstOrThrowAsync(c => c.OAuhtId == auth0Id);

        appDbContext.Accounts.Remove(account);
        await appDbContext.SaveChangesAsync();
    }
}
