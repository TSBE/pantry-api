using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Queries;

public class HouseholdQueryHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<HouseholdQueryHandler> _logger;

    private readonly IPrincipal _principal;

    public HouseholdQueryHandler(
        ILogger<HouseholdQueryHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<Household> ExecuteAsync(HouseholdQuery query)
    {
        _logger.ExecutingQuery(nameof(HouseholdQuery));

        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var householdId = _principal.GetHouseholdIdOrThrow();
        return await appDbContext.Households.AsNoTracking().FirstOrThrowAsync(c => c.HouseholdId == householdId);
    }
}
