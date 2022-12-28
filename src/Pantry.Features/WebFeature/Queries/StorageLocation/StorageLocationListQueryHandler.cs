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

public class StorageLocationListQueryHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<StorageLocationListQueryHandler> _logger;

    private readonly IPrincipal _principal;

    public StorageLocationListQueryHandler(
        ILogger<StorageLocationListQueryHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<IReadOnlyCollection<StorageLocation>> ExecuteAsync(StorageLocationListQuery query)
    {
        _logger.ExecutingQuery(nameof(StorageLocationListQuery));

        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var householdId = _principal.GetHouseholdIdOrThrow();
        return await appDbContext.StorageLocations.Include(i => i.Household).AsNoTracking().Where(c => c.HouseholdId == householdId).ToListAsync();
    }
}
