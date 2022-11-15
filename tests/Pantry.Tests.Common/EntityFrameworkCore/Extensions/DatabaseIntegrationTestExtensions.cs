using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pantry.Tests.Common;
using Pantry.Tests.EntityFrameworkCore.Persistence;

namespace Pantry.Tests.EntityFrameworkCore.Extensions;

/// <summary>
///     Extensions to make the handling of Database in TestApplication easier.
/// </summary>
public static class DatabaseIntegrationTestExtensions
{
    /// <summary>
    ///     Replaces any Database already configure with a shared in-memory sqlite database.
    /// </summary>
    /// <typeparam name="TContext">The DbContext Type to be replaced.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection" /> to operate on.</param>
    /// <param name="configurationAction">An optional action that allows for additional configuration of the <see cref="DbContextOptionsBuilder"/>.</param>
    /// <returns>The <see cref="IServiceCollection" /> to chain calls.</returns>
    public static IServiceCollection OverrideWithSharedInMemorySqliteDatabase<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder>? configurationAction = null)
        where TContext : DbContext
    {
        services.RemoveAll<DbContextOptions<TContext>>();
        services.RemoveAll<TContext>();

        services.AddSingleton<SharedInmemorySqliteDatabaseProvider<TContext>>();

        void OptionBuilder(IServiceProvider serviceProvider, DbContextOptionsBuilder options)
        {
            SharedInmemorySqliteDatabaseProvider<TContext> sqliteDatabaseProvider = serviceProvider.GetRequiredService<SharedInmemorySqliteDatabaseProvider<TContext>>();

            options = options.UseSqlite(sqliteDatabaseProvider.ConnectionString)
                .EnableDetailedErrors();

            configurationAction?.Invoke(options);
        }

        services.AddPooledDbContextFactory<TContext>(OptionBuilder);

        // This is just required for the HealthCheck, since it does not (yet?) work with the Factory.
        services.AddDbContextPool<TContext>(OptionBuilder);

        return services;
    }

    /// <summary>
    ///     Replaces any Database already configure with a shared in-memory sqlite database.
    /// </summary>
    /// <typeparam name="TContext">The DbContext Type to be replaced.</typeparam>
    /// <typeparam name="TFactory">A custom factory for the creation of the context.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection" /> to operate on.</param>
    /// <param name="configurationAction">An optional action that allows for additional configuration of the <see cref="DbContextOptionsBuilder"/>.</param>
    /// <returns>The <see cref="IServiceCollection" /> to chain calls.</returns>
    public static IServiceCollection OverrideWithSharedInMemorySqliteDatabase<TContext, TFactory>(this IServiceCollection services, Action<DbContextOptionsBuilder>? configurationAction = null)
        where TContext : DbContext
        where TFactory : IDbContextFactory<TContext>
    {
        services.RemoveAll<DbContextOptions<TContext>>();
        services.RemoveAll<TContext>();

        services.AddSingleton<SharedInmemorySqliteDatabaseProvider<TContext>>();
        services.AddDbContextFactory<TContext, TFactory>(OptionBuilder);

        // This is just required for the HealthCheck, since it does not (yet?) work with the Factory.
        services.AddDbContextPool<TContext>(OptionBuilder);

        void OptionBuilder(IServiceProvider serviceProvider, DbContextOptionsBuilder options)
        {
            SharedInmemorySqliteDatabaseProvider<TContext> sqliteDatabaseProvider = serviceProvider.GetRequiredService<SharedInmemorySqliteDatabaseProvider<TContext>>();

            options = options.UseSqlite(sqliteDatabaseProvider.ConnectionString)
                .EnableDetailedErrors();

            configurationAction?.Invoke(options);
        }

        return services;
    }

    /// <summary>
    ///     Ensures that the Database is created and optionally executes some initialization.
    /// </summary>
    /// <typeparam name="TContext">The DbContext Type to set up.</typeparam>
    /// <param name="app">The TestApplication.</param>
    /// <param name="setupAction">An optional SetupAction to be performed after the Database is created.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    public static async Task SetupDatabaseAsync<TContext>(this ITestApplication app, Action<TContext>? setupAction = null)
        where TContext : DbContext
    {
        using IServiceScope scope = app.Services.CreateScope();

        IDbContextFactory<TContext> dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<TContext>>();
        using (TContext appDbContext = dbContextFactory.CreateDbContext())
        {
            await appDbContext.Database.EnsureCreatedAsync();

            if (setupAction != null)
            {
                setupAction(appDbContext);
                await appDbContext.SaveChangesAsync();
            }
        }
    }

    /// <summary>
    ///     Ensures that the Database is created and optionally executes some initialization.
    /// </summary>
    /// <typeparam name="TContext">The DbContext Type to set up.</typeparam>
    /// <param name="app">The TestApplication.</param>
    /// <param name="setupAction">An optional SetupAction to be performed after the Database is created.</param>
    public static void SetupDatabase<TContext>(this ITestApplication app, Action<TContext>? setupAction = null)
        where TContext : DbContext
    {
        using IServiceScope scope = app.Services.CreateScope();

        IDbContextFactory<TContext> dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<TContext>>();
        using (TContext appDbContext = dbContextFactory.CreateDbContext())
        {
            appDbContext.Database.EnsureCreated();

            if (setupAction != null)
            {
                setupAction(appDbContext);
                appDbContext.SaveChanges();
            }
        }
    }

    /// <summary>
    ///     Verifies assertion on the database registered in the TestApplication.
    /// </summary>
    /// <typeparam name="TContext">The DbContext Type to assert on.</typeparam>
    /// <param name="app">The TestApplication.</param>
    /// <param name="assertAction">The assertions to be performed.</param>
    public static void AssertDatabaseContent<TContext>(this ITestApplication app, Action<TContext> assertAction)
        where TContext : DbContext
    {
        using IServiceScope scope = app.Services.CreateScope();

        IDbContextFactory<TContext> dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<TContext>>();
        using (TContext appDbContext = dbContextFactory.CreateDbContext())
        {
            assertAction(appDbContext);
        }
    }
}
