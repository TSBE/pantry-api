using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Queries;

public class DeviceByIdQueryHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<DeviceByIdQueryHandler> _logger;

    public DeviceByIdQueryHandler(ILogger<DeviceByIdQueryHandler> logger, IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Device> ExecuteAsync(DeviceByIdQuery query)
    {
        _logger.ExecutingQuery(nameof(DeviceByIdQuery));

        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();
        return await appDbContext.Devices
            .AsNoTracking().FirstOrThrowAsync(c => c.DeviceId == query.DeviceId);
    }
}
