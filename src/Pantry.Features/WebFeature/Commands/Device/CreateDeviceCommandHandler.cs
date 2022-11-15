using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Commands;

public class CreateDeviceCommandHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<CreateDeviceCommandHandler> _logger;

    public CreateDeviceCommandHandler(
        ILogger<CreateDeviceCommandHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Device> ExecuteAsync(CreateDeviceCommand command)
    {
        _logger.ExecutingCommand(nameof(CreateDeviceCommand));
        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var device = new Device
        {
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
