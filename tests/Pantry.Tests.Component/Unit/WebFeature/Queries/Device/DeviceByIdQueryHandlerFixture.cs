using Microsoft.Extensions.Logging;
using Pantry.Common.EntityFrameworkCore.Exceptions;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Queries;

namespace Pantry.Tests.Component.Unit.WebFeature.Queries;

[Trait("Category", "Unit")]
public class DeviceByIdQueryHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturn()
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

        var queryHandler = new DeviceByIdQueryHandler(Substitute.For<ILogger<DeviceByIdQueryHandler>>(), testDatabase, PrincipalOfJohnDoe);

        // Act
        Device actual = await queryHandler.ExecuteAsync(new DeviceByIdQuery(device.InstallationId));

        // Assert
        actual.InstallationId.ShouldBe(device.InstallationId);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow()
    {
        // Arrange
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase();

        var queryHandler = new DeviceByIdQueryHandler(Substitute.For<ILogger<DeviceByIdQueryHandler>>(), testDatabase, PrincipalOfJohnDoe);

        // Act
        Func<Task> act = async () => await queryHandler.ExecuteAsync(new DeviceByIdQuery(Guid.NewGuid()));

        // Assert
        await act.ShouldThrowAsync<EntityNotFoundException>();
    }
}
