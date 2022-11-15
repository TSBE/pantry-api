using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Pantry.Common.EntityFrameworkCore.Migrations;

/// <summary>
///     <see cref="IDatabaseMigrationStrategy{TDbContext}"/> implementation that uses EF Core migrations
///     to migrate the database.
/// </summary>
/// <typeparam name="TDbContext">The <see cref="DbContext"/> type.</typeparam>
public class EfCoreMigrationsMigrationStrategy<TDbContext> : IDatabaseMigrationStrategy<TDbContext>
    where TDbContext : DbContext
{
    private readonly ILogger<EfCoreMigrationsMigrationStrategy<TDbContext>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EfCoreMigrationsMigrationStrategy{TDbContext}"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public EfCoreMigrationsMigrationStrategy(ILogger<EfCoreMigrationsMigrationStrategy<TDbContext>> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public void MigrateDatabase(TDbContext dbContext, CancellationToken stoppingToken)
    {
        // We run it synchronously to prevent starting of other services
        dbContext.Database.MigrateWithRetriesAsync(_logger, cancellationToken: stoppingToken)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }
}
