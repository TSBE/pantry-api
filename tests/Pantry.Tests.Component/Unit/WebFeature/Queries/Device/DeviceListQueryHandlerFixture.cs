using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Queries;

namespace Pantry.Tests.Component.Unit.WebFeature.Queries;

[Trait("Category", "Unit")]
public class DeviceListQueryHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturn()
    {
        // Arrange
        var device1 = new Device { Account = AccountJohnDoe, InstallationId = Guid.NewGuid(), Name = "iPhone 14", Model = "Foo`s iPhone", Platform = Core.Persistence.Enums.DevicePlatformType.IOS };
        var device2 = new Device { Account = AccountJohnDoe, InstallationId = Guid.NewGuid(), Name = "Samsung Galaxy S22", Model = "Foo`s Galaxy", Platform = Core.Persistence.Enums.DevicePlatformType.ANDROID };

        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Devices.Add(device1);
            dbContext.Devices.Add(device2);
        });

        var queryHandler = new DeviceListQueryHandler(
            Substitute.For<ILogger<DeviceListQueryHandler>>(),
            testDatabase,
            PrincipalOfJohnDoe);

        // Act
        IReadOnlyCollection<Device> devices = await queryHandler.ExecuteAsync(new DeviceListQuery());

        // Assert
        devices.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmpty()
    {
        // Arrange
        var device1 = new Device { Account = AccountJohnDoe, InstallationId = Guid.NewGuid(), Name = "iPhone 14", Model = "Foo`s iPhone", Platform = Core.Persistence.Enums.DevicePlatformType.IOS };
        var device2 = new Device { Account = AccountJohnDoe, InstallationId = Guid.NewGuid(), Name = "Samsung Galaxy S22", Model = "Foo`s Galaxy", Platform = Core.Persistence.Enums.DevicePlatformType.ANDROID };

        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Devices.Add(device1);
            dbContext.Devices.Add(device2);
        });

        var queryHandler = new DeviceListQueryHandler(
            Substitute.For<ILogger<DeviceListQueryHandler>>(),
            testDatabase,
            PrincipalAuthenticatedUser1);

        // Act
        IReadOnlyCollection<Device> devices = await queryHandler.ExecuteAsync(new DeviceListQuery());

        // Assert
        devices.Count.ShouldBe(0);
    }
}
