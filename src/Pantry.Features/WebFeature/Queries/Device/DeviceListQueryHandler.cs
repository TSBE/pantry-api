using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Queries;

public class DeviceListQueryHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<DeviceListQueryHandler> _logger;

    public DeviceListQueryHandler(
        ILogger<DeviceListQueryHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IReadOnlyCollection<Device>> ExecuteAsync(DeviceListQuery query)
    {
        _logger.ExecutingQuery(nameof(DeviceListQuery));

        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();
        return await appDbContext.Devices.AsNoTracking().ToListAsync();
    }
}
