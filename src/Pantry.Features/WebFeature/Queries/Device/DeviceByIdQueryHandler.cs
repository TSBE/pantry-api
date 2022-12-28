using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Queries;

public class DeviceByIdQueryHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<DeviceByIdQueryHandler> _logger;

    private readonly IPrincipal _principal;

    public DeviceByIdQueryHandler(
        ILogger<DeviceByIdQueryHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<Device> ExecuteAsync(DeviceByIdQuery query)
    {
        _logger.ExecutingQuery(nameof(DeviceByIdQuery));

        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var auth0Id = _principal.GetAuth0IdOrThrow();
        return await appDbContext.Devices.Include(x => x.Account).AsNoTracking().FirstOrThrowAsync(c => c.Account.OAuhtId == auth0Id && c.InstallationId == query.InstallationId);
    }
}
