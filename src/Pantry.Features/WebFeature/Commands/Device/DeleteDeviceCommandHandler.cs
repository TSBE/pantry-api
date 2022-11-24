using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Commands;

public class DeleteDeviceCommandHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<DeleteDeviceCommandHandler> _logger;

    private readonly IPrincipal _principal;

    public DeleteDeviceCommandHandler(
        ILogger<DeleteDeviceCommandHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task ExecuteAsync(DeleteDeviceCommand command)
    {
        _logger.ExecutingCommand(nameof(DeleteDeviceCommand));
        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var auth0Id = _principal.GetAuth0IdOrThrow();
        var device = await appDbContext.Devices.Include(x => x.Account).FirstOrThrowAsync(c => c.Account.OAuhtId == auth0Id && c.InstallationId == command.InstallationId);

        appDbContext.Devices.Remove(device);
        await appDbContext.SaveChangesAsync();
    }
}
