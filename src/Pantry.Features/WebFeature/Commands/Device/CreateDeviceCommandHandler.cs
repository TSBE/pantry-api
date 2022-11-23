using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Commands;

public class CreateDeviceCommandHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<CreateDeviceCommandHandler> _logger;

    private readonly IPrincipal _principal;

    public CreateDeviceCommandHandler(
        ILogger<CreateDeviceCommandHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<Device> ExecuteAsync(CreateDeviceCommand command)
    {
        _logger.ExecutingCommand(nameof(CreateDeviceCommand));
        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var auth0Id = _principal.GetAuth0IdOrThrow();
        var account = await appDbContext.Accounts.FirstOrThrowAsync(c => c.OAuhtId == auth0Id);

        var device = new Device
        {
            Account = account,
            DeviceToken = command.DeviceToken,
            InstallationId = command.InstallationId,
            Model = command.Model,
            Name = command.Name,
            Platform = command.Platform
        };

        appDbContext.Devices.Add(device);
        await appDbContext.SaveChangesAsync();

        return device;
    }
}
