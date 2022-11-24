using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Commands;

public class UpdateStorageLocationCommandHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<UpdateStorageLocationCommandHandler> _logger;

    private readonly IPrincipal _principal;

    public UpdateStorageLocationCommandHandler(
        ILogger<UpdateStorageLocationCommandHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<StorageLocation> ExecuteAsync(UpdateStorageLocationCommand command)
    {
        _logger.ExecutingCommand(nameof(UpdateStorageLocationCommand));
        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var householdId = _principal.GetHouseholdIdOrThrow();
        var storageLocation = await appDbContext.StorageLocations.Include(i => i.Household).FirstOrThrowAsync(c => c.HouseholdId == householdId && c.StorageLocationId == command.StorageLocationId);

        storageLocation.Name = command.Name;
        storageLocation.Description = command.Description;

        await appDbContext.SaveChangesAsync();

        return storageLocation;
    }
}
