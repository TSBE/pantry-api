using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Pantry.Common.EntityFrameworkCore.Migrations;

/// <summary>
///     <see cref="IDatabaseMigrationStrategy{TDbContext}"/> implementation that uses
///     <see cref="DatabaseFacade.EnsureCreated"/> to migrate the database.
/// </summary>
/// <typeparam name="TDbContext">The <see cref="DbContext"/> type.</typeparam>
public class EnsureDatabaseCreatedMigrationStrategy<TDbContext> : IDatabaseMigrationStrategy<TDbContext>
    where TDbContext : DbContext
{
    /// <inheritdoc/>
    public void MigrateDatabase(TDbContext dbContext, CancellationToken stoppingToken)
    {
        dbContext.Database.EnsureCreated();
    }
}
