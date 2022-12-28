using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Commands;

public class DeleteStorageLocationCommandHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<DeleteStorageLocationCommandHandler> _logger;

    private readonly IPrincipal _principal;

    public DeleteStorageLocationCommandHandler(
        ILogger<DeleteStorageLocationCommandHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task ExecuteAsync(DeleteStorageLocationCommand command)
    {
        _logger.ExecutingCommand(nameof(DeleteStorageLocationCommand));
        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var householdId = _principal.GetHouseholdIdOrThrow();
        var storageLocation = await appDbContext.StorageLocations.Include(i => i.Household).FirstOrThrowAsync(c => c.HouseholdId == householdId && c.StorageLocationId == command.StorageLocationId);
        appDbContext.StorageLocations.Remove(storageLocation);
        await appDbContext.SaveChangesAsync();
    }
}
