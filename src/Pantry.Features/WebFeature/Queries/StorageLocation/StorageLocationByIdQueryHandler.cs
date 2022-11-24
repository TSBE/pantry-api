using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Queries;

public class StorageLocationByIdQueryHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<StorageLocationByIdQueryHandler> _logger;

    private readonly IPrincipal _principal;

    public StorageLocationByIdQueryHandler(
        ILogger<StorageLocationByIdQueryHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<StorageLocation> ExecuteAsync(StorageLocationByIdQuery query)
    {
        _logger.ExecutingQuery(nameof(StorageLocationByIdQuery));

        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var householdId = _principal.GetHouseholdIdOrThrow();
        return await appDbContext.StorageLocations.Include(i => i.Household).AsNoTracking().FirstOrThrowAsync(c => c.HouseholdId == householdId && c.StorageLocationId == query.StorageLocationId);
    }
}
