using Microsoft.Extensions.Logging;

namespace Pantry.Common.EntityFrameworkCore.Migrations;

internal static class LoggerExtensions
{
    private const string FeatureName = "EFCoreMigrations";

    public static void MigrationsSucceeded(this ILogger logger, int attempt, int totalAttempts)
    {
        logger.LogInformation(
            new EventId(1, FeatureName),
            "Migration of the database succeeded. Attempt {Attempt} of {TotalAttempts}",
            attempt,
            totalAttempts);
    }

    public static void MigrationsAttemptFailed(this ILogger logger, Exception exception, int attempt, int totalAttempts)
    {
        logger.LogWarning(
            new EventId(2, FeatureName),
            exception,
            "Migration of the database failed. Attempt {Attempt} of {TotalAttempts}",
            attempt,
            totalAttempts);
    }
}
