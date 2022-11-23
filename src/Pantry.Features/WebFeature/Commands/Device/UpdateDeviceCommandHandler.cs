using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Commands;

public class UpdateDeviceCommandHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<UpdateDeviceCommandHandler> _logger;

    private readonly IPrincipal _principal;

    public UpdateDeviceCommandHandler(
        ILogger<UpdateDeviceCommandHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<Device> ExecuteAsync(UpdateDeviceCommand command)
    {
        _logger.ExecutingCommand(nameof(UpdateDeviceCommand));
        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var auth0Id = _principal.GetAuth0IdOrThrow();
        var device = await appDbContext.Devices.Include(x => x.Account).FirstOrThrowAsync(c => c.Account.OAuhtId == auth0Id && c.InstallationId == command.InstallationId);

        device.Name = command.Name;
        device.DeviceToken = command.DeviceToken;

        await appDbContext.SaveChangesAsync();

        return device;
    }
}
