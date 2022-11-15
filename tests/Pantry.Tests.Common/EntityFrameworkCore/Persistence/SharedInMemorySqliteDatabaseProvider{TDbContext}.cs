using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Pantry.Tests.EntityFrameworkCore.Persistence;

/// <summary>
///     On instantiation of this class a connection to a new SQLite in-memory database is opened.
///     To use the new in-memory database use the connection string provided by <see cref="ConnectionString" />.
///     On disposing of the instance the initial connection is disposed as well and the database destroyed (if there are no other
///     connections to it).
///     Usage:
///     This class is meant to be registered as a singleton with the ServiceProvider. Whenever the connection string is needed (eg.
///     to create a new DbContext) get the Instance of this Singleton from the ServiceProvider and use the
///     <see cref="ConnectionString" /> property. Make sure to dispose the ServiceProvider at the end, to make sure that the database
///     gets destroyed.
/// </summary>
/// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
public sealed class SharedInmemorySqliteDatabaseProvider<TDbContext> : IDisposable
    where TDbContext : DbContext
{
    // This connection ensures that the database is preserved for the whole duration
    // of the integration test. The database is deleted as soon as all connections
    // are closed.
    private readonly SqliteConnection _keepAliveConnection;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SharedInmemorySqliteDatabaseProvider{TDbContext}" /> class.
    /// </summary>
    public SharedInmemorySqliteDatabaseProvider()
    {
        // Create a unique name for the database
        string databaseName = Guid.NewGuid().ToString("N");

        // By using shared cache we can have multiple connections to the very same
        // in memory SQLite database.
        ConnectionString = $"DataSource={databaseName};mode=memory;cache=shared";

        _keepAliveConnection = new SqliteConnection(ConnectionString);
        _keepAliveConnection.Open();
    }

    /// <summary>
    ///     The ConnectionString to be used to connect to the in memory database.
    /// </summary>
    public string ConnectionString { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        _keepAliveConnection.Close();
        _keepAliveConnection.Dispose();
    }
}
