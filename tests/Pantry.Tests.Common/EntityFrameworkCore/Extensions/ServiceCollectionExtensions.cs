using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Pantry.Tests.EntityFrameworkCore.Extensions;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Override the existing <see cref="DbContext"/> registration with one that
    /// uses a SQLite database.
    /// </summary>
    /// <typeparam name="TDbContext">The type of DbContext to override.</typeparam>
    /// <typeparam name="TImplementationContext">The type of the DbContext to use.</typeparam>
    /// <param name="services">A service collection.</param>
    /// <param name="connection">The SQLite connection to use.</param>
    /// <param name="loggerFactory">An optional <see cref="ILoggerFactory"/> to use.</param>
    /// <param name="optionsAction">An optional <see cref="Action"/> to further configure the <see cref="DbContextOptions{TContext}"/>.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection OverrideWithInMemorySqliteDatabase<TDbContext, TImplementationContext>(
        this IServiceCollection services,
        SqliteConnection connection,
        ILoggerFactory? loggerFactory = null,
        Action<DbContextOptionsBuilder>? optionsAction = null)
        where TDbContext : DbContext
        where TImplementationContext : TDbContext
    {
        services.RemoveAll<TDbContext>();

        services.AddDbContextPool<TDbContext, TImplementationContext>((serviceProvider, optionsBuilder) =>
        {
            optionsBuilder.UseSqlite(connection)
                .EnableDetailedErrors();

            if (loggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(loggerFactory)
                    .EnableSensitiveDataLogging();
            }

            optionsAction?.Invoke(optionsBuilder);
        });

        return services;
    }
}
