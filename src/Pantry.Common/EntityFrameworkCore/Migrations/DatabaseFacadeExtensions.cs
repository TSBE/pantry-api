using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Pantry.Common.EntityFrameworkCore.Migrations;

/// <summary>
/// Extensions for <see cref="DatabaseFacade"/>.
/// </summary>
public static class DatabaseFacadeExtensions
{
    /// <summary>
    ///     <para>
    ///         Asynchronously applies any pending migrations for the context to the database. Will create the database
    ///         if it does not already exist.
    ///     </para>
    ///     <para>
    ///         CAUTION: This method will do retries if the migrations fail. Therefore use this method with caution. It is not intended
    ///         for usage in production scenarios but rather for local development.
    ///     </para>
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="maxRetries">The maximum number of retries.</param>
    /// <param name="delay">The delay between each retry. Default is 5 seconds.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous migration operation.</returns>
    public static async Task MigrateWithRetriesAsync(this DatabaseFacade database, ILogger logger, int maxRetries = 5, TimeSpan? delay = null, CancellationToken cancellationToken = default)
    {
        TimeSpan appliedDelay = delay ?? TimeSpan.FromSeconds(5);

        for (var i = 1; i <= maxRetries; i++)
        {
            try
            {
                await database.MigrateAsync(cancellationToken);
                logger.MigrationsSucceeded(i, maxRetries);
                return;
            }
            catch (SqlException ex)
            {
                if (i < maxRetries)
                {
                    logger.MigrationsAttemptFailed(ex, i, maxRetries);
                    await Task.Delay(appliedDelay, cancellationToken);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
