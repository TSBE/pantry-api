using Microsoft.Extensions.Logging;
using Pantry.Common.EntityFrameworkCore.Exceptions;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Commands;

namespace Pantry.Tests.Component.Unit.WebFeature.Commands;

[Trait("Category", "Unit")]
public class UpdateDeviceCommandHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldUpdateDevice()
    {
        // Arrange
        var device = new Device { Account = AccountJohnDoe, InstallationId = Guid.NewGuid(), Name = "iPhone 14", Model = "Foo`s iPhone", Platform = Core.Persistence.Enums.DevicePlatformType.IOS };
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Devices.Add(device);
            });
        var commandHandler = new UpdateDeviceCommandHandler(
            Substitute.For<ILogger<UpdateDeviceCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoe);

        // Act
        var act = await commandHandler.ExecuteAsync(new UpdateDeviceCommand(device.InstallationId, "Bar`s iPhone", "unittesttoken"));

        // Assert
        act.Name.ShouldBe("Bar`s iPhone");
        act.DeviceToken.ShouldBe("unittesttoken");
        act.InstallationId.ShouldBe(device.InstallationId);
        act.Model.ShouldBeEquivalentTo(device.Model);
        act.Platform.ShouldBe(device.Platform);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow()
    {
        // Arrange
        var device = new Device { Account = AccountJohnDoe, InstallationId = Guid.NewGuid(), Name = "iPhone 14", Model = "Foo`s iPhone", Platform = Core.Persistence.Enums.DevicePlatformType.IOS };
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Devices.Add(device);
            });
        var commandHandler = new UpdateDeviceCommandHandler(
            Substitute.For<ILogger<UpdateDeviceCommandHandler>>(),
            testDatabase,
            PrincipalAuthenticatedUser1);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new UpdateDeviceCommand(device.InstallationId, "Hacker's Device", null));

        // Assert
        await act.ShouldThrowAsync<EntityNotFoundException>();
    }
}
