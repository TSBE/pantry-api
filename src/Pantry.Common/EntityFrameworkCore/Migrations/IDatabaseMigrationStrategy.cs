using Microsoft.EntityFrameworkCore;

namespace Pantry.Common.EntityFrameworkCore.Migrations;

/// <summary>
///     A strategy to bring the database to the state the application
///     requires to run.
/// </summary>
/// <typeparam name="TDbContext">The <see cref="DbContext"/> type.</typeparam>
public interface IDatabaseMigrationStrategy<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    ///     Run the database migration.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/> to run the migrations against.</param>
    /// <param name="stoppingToken">The cancellation token.</param>
    void MigrateDatabase(TDbContext dbContext, CancellationToken stoppingToken);
}
