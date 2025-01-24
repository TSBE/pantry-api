using Microsoft.Extensions.Logging;
using Pantry.Common.EntityFrameworkCore.Exceptions;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Commands;

namespace Pantry.Tests.Component.Unit.WebFeature.Commands;

[Trait("Category", "Unit")]
public class UpdateStorageLocationCommandHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldUpdateStorageLocation()
    {
        // Arrange
        var storageLocation = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 1, Name = "Unittest Location", Description = "Test Description" };
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.StorageLocations.Add(storageLocation);
        });
        var commandHandler = new UpdateStorageLocationCommandHandler(
            Substitute.For<ILogger<UpdateStorageLocationCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoeWithHousehold);

        // Act
        var act = await commandHandler.ExecuteAsync(new UpdateStorageLocationCommand(storageLocation.StorageLocationId, "Updated Location", "Updated Description"));

        // Assert
        act.Name.ShouldBe("Updated Location");
        act.Description.ShouldBe("Updated Description");
        act.HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow()
    {
        // Arrange
        var storageLocation = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 1, Name = "Unittest Location", Description = "Test Description" };
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Accounts.Add(AccountFooBar);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.Households.Add(HouseholdOfFooBar);
            dbContext.StorageLocations.Add(storageLocation);
        });
        var commandHandler = new UpdateStorageLocationCommandHandler(
            Substitute.For<ILogger<UpdateStorageLocationCommandHandler>>(),
            testDatabase,
            PrincipalOfFooBarWithHousehold);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new UpdateStorageLocationCommand(storageLocation.StorageLocationId, "Updated Location", "Updated Description"));

        // Assert
        await act.ShouldThrowAsync<EntityNotFoundException>();
    }
}
