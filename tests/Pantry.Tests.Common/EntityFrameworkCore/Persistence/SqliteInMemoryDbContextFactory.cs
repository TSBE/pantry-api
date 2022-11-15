using Microsoft.EntityFrameworkCore;

namespace Pantry.Tests.EntityFrameworkCore.Persistence;

/// <summary>
///     This class can be used to create a relational SqLite InMemory Database for testing purposes.
/// </summary>
/// <remarks>
///     On instantiation of this class a Connection to a new SqLite InMemory Database is opened.
///     On disposing of the instance the initial connection is disposed as well and the database destroyed (if there are no other
///     connections to it).
/// </remarks>
/// <typeparam name="TDbContext">Derived type of a <see cref="DbContext" /> class.</typeparam>
public sealed class SqliteInMemoryDbContextFactory<TDbContext> : IDbContextFactory<TDbContext>, IDisposable
    where TDbContext : DbContext
{
    private readonly SharedInmemorySqliteDatabaseProvider _sharedInmemorySqliteDatabaseProvider;
    private readonly Func<DbContextOptions, TDbContext> _createContext;
    private readonly DbContextOptions _dbContextOptions;
    private bool _isDisposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SqliteInMemoryDbContextFactory{TDbContext}"/> class.
    /// </summary>
    public SqliteInMemoryDbContextFactory(Action<DbContextOptionsBuilder<TDbContext>>? optionsAction = null, Func<DbContextOptions, TDbContext>? createContext = null)
    {
        _sharedInmemorySqliteDatabaseProvider = new SharedInmemorySqliteDatabaseProvider();
        DbContextOptionsBuilder<TDbContext> optionsBuilder = new DbContextOptionsBuilder<TDbContext>()
                .UseSqlite(_sharedInmemorySqliteDatabaseProvider.ConnectionString)
                .EnableSensitiveDataLogging();
        optionsAction?.Invoke(optionsBuilder);
        _dbContextOptions = optionsBuilder.Options;

        _createContext = createContext ?? new Func<DbContextOptions, TDbContext>(options => (TDbContext)Activator.CreateInstance(typeof(TDbContext), options)!);
    }

    /// <summary>
    ///     Gets the database connection string.
    /// </summary>
    internal string DbConnectionString => _sharedInmemorySqliteDatabaseProvider.ConnectionString;

    /// <summary>
    ///     Creates a new database context object.
    /// </summary>
    /// <returns>An instance of a derived <see cref="DbContext" /> class.</returns>
    public TDbContext CreateDbContext()
    {
        return _createContext(_dbContextOptions);
    }

    /// <inheritdoc />
    public void Dispose() => Dispose(true);

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // Dispose managed resources.
                _sharedInmemorySqliteDatabaseProvider.Dispose();
            }

            // Release unmanaged resources.
            _isDisposed = true;
        }
    }
}
