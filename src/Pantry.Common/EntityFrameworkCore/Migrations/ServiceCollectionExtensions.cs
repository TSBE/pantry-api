using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Pantry.Common.EntityFrameworkCore.Migrations;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Runs the EF Core migrations in a <see cref="IHostedService"/>.
    /// Uses <see cref="DatabaseFacadeExtensions.MigrateWithRetriesAsync"/> internally.
    ///
    /// <remarks>
    ///     Make sure that you call this before any other <see cref="IHostedService"/> that
    ///     needs the database.
    ///     Use this service with caution. See <see cref="DatabaseFacadeExtensions.MigrateWithRetriesAsync"/>
    ///     for further information regarding when to use this functionality.
    /// </remarks>
    /// </summary>
    /// <typeparam name="TDbContext">The type of the database context.</typeparam>
    /// <param name="services">The services.</param>
    public static void AddDatabaseMigrationHostedService<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddHostedService<DatabaseMigrationHostedService<TDbContext>>();
        services.AddSingleton<IDatabaseMigrationStrategy<TDbContext>, EfCoreMigrationsMigrationStrategy<TDbContext>>();
    }

    /// <summary>
    /// Ensures that the database is created using a <see cref="IHostedService"/>.
    /// Uses <see cref="DatabaseFacade.EnsureCreated"/> internally.
    ///
    /// <remarks>
    ///     Make sure that you call this before any other <see cref="IHostedService"/> that
    ///     needs the database.
    /// </remarks>
    /// </summary>
    /// <typeparam name="TDbContext">The type of the database context.</typeparam>
    /// <param name="services">The services.</param>
    public static void AddEnsureDatabaseCreatedHostedService<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddHostedService<DatabaseMigrationHostedService<TDbContext>>();
        services.AddSingleton<IDatabaseMigrationStrategy<TDbContext>, EnsureDatabaseCreatedMigrationStrategy<TDbContext>>();
    }
}
