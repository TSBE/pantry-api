using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantry.Common.Authentication;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Diagnostics;

namespace Pantry.Features.WebFeature.Commands;

public class CreateStorageLocationCommandHandler
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    private readonly ILogger<CreateStorageLocationCommandHandler> _logger;

    private readonly IPrincipal _principal;

    public CreateStorageLocationCommandHandler(
        ILogger<CreateStorageLocationCommandHandler> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        IPrincipal principal)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _principal = principal;
    }

    public async Task<StorageLocation> ExecuteAsync(CreateStorageLocationCommand command)
    {
        _logger.ExecutingCommand(nameof(CreateStorageLocationCommand));
        using AppDbContext appDbContext = _dbContextFactory.CreateDbContext();

        var householdId = _principal.GetHouseholdIdOrThrow();
        var household = await appDbContext.Households.FirstOrThrowAsync(c => c.HouseholdId == householdId);

        var storageLocation = new StorageLocation
        {
            Household = household,
            Name = command.Name,
            Description = command.Description
        };

        appDbContext.StorageLocations.Add(storageLocation);
        await appDbContext.SaveChangesAsync();

        return storageLocation;
    }
}
