using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Queries;

public class DeviceListQueryHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<DeviceListQueryHandler> _logger;

    private readonly IPrincipal _principal;

    public DeviceListQueryHandler(
        ILogger<DeviceListQueryHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<IReadOnlyCollection<Device>> ExecuteAsync(DeviceListQuery query)
    {
        _logger.ExecutingQuery(nameof(DeviceListQuery));

        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var auth0Id = _principal.GetAuth0IdOrThrow();
        return await appDbContext.Devices.Include(x => x.Account).AsNoTracking().Where(c => c.Account.OAuhtId == auth0Id).ToListAsync();
    }
}
