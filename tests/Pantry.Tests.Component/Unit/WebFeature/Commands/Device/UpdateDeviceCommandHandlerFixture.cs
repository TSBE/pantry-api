using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pantry.Common.EntityFrameworkCore.Exceptions;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Commands;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;

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
        act.Name.Should().Be("Bar`s iPhone");
        act.DeviceToken.Should().Be("unittesttoken");
        act.InstallationId.Should().Be(device.InstallationId);
        act.Model.Should().BeEquivalentTo(device.Model);
        act.Platform.Should().Be(device.Platform);
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
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }
}
