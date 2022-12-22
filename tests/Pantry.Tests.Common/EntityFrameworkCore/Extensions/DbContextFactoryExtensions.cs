using Microsoft.EntityFrameworkCore;

namespace Pantry.Tests.EntityFrameworkCore.Extensions;

/// <summary>
///     Extensions to provide helper methods for tests which uses a database.
/// </summary>
public static class DbContextFactoryExtensions
{
    /// <summary>
    ///     Ensures that the Database is created and optionally executes some initialization routine.
    /// </summary>
    /// <typeparam name="TDbContext">The derived type of a <see cref="DbContext"/> class to set up.</typeparam>
    /// <param name="dbContextFactory">An implementation of a <see cref="IDbContextFactory{TContext}" /> interface.</param>
    /// <param name="setupAction">An optional <see cref="Action" /> to be performed after the Database is created.</param>
    public static void SetupDatabase<TDbContext>(this IDbContextFactory<TDbContext> dbContextFactory, Action<TDbContext>? setupAction = null)
        where TDbContext : DbContext
    {
        using TDbContext dbContext = dbContextFactory.CreateDbContext();
        dbContext.Database.EnsureCreated();

        if (setupAction != null)
        {
            setupAction(dbContext);
            dbContext.SaveChanges();
        }
    }

    /// <summary>
    ///     Performs a assertion routine to verify the results from the database.
    /// </summary>
    /// <typeparam name="TDbContext">The derived type of a <see cref="DbContext"/> class to set up.</typeparam>
    /// <param name="dbContextFactory">An implementation of a <see cref="IDbContextFactory{TContext}" /> interface.</param>
    /// <param name="assertAction">An <see cref="Action" /> to verify assertion on the database.</param>
    public static void AssertDatabaseContent<TDbContext>(this IDbContextFactory<TDbContext> dbContextFactory, Action<TDbContext> assertAction)
        where TDbContext : DbContext
    {
        using TDbContext dbContext = dbContextFactory.CreateDbContext();
        assertAction.Invoke(dbContext);
    }
}
