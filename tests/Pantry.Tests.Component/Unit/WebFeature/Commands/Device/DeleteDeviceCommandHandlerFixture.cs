﻿using Microsoft.Extensions.Logging;
using Pantry.Common.EntityFrameworkCore.Exceptions;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Commands;

namespace Pantry.Tests.Component.Unit.WebFeature.Commands;

[Trait("Category", "Unit")]
public class DeleteDeviceCommandHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldDeleteDevice()
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
        var commandHandler = new DeleteDeviceCommandHandler(
            Substitute.For<ILogger<DeleteDeviceCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoe);

        // Act
        await commandHandler.ExecuteAsync(new DeleteDeviceCommand(device.InstallationId));

        // Assert
        testDatabase.AssertDatabaseContent(
            dbContext =>
            {
                dbContext.Accounts.Count().ShouldBe(1);
                dbContext.Devices.Count().ShouldBe(0);
            });
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow()
    {
        // Arrange
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase();
        var commandHandler = new DeleteDeviceCommandHandler(
            Substitute.For<ILogger<DeleteDeviceCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoe);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new DeleteDeviceCommand(Guid.NewGuid()));

        // Assert
        await act.ShouldThrowAsync<EntityNotFoundException>();
    }
}
