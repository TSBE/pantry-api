using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pantry.Common.EntityFrameworkCore.Migrations;

internal class DatabaseMigrationHostedService<TDbContext> : BackgroundService
    where TDbContext : DbContext
{
    private readonly IDatabaseMigrationStrategy<TDbContext> _databaseMigrationStrategy;

    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly ILogger<DatabaseMigrationHostedService<TDbContext>> _logger;

    public DatabaseMigrationHostedService(
        IDatabaseMigrationStrategy<TDbContext> databaseMigrationStrategy,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<DatabaseMigrationHostedService<TDbContext>> logger)
    {
        _databaseMigrationStrategy = databaseMigrationStrategy;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(new EventId(10000, "StartingDatabaseMigrationService"), "Starting Database Migration hosted service");

        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();

        TDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<TDbContext>();

        _databaseMigrationStrategy.MigrateDatabase(dbContext, stoppingToken);

        _logger.LogInformation(new EventId(10001, "StoppingDatabaseMigrationService"), "Stopping Database Migration hosted service");

        return Task.CompletedTask;
    }
}
