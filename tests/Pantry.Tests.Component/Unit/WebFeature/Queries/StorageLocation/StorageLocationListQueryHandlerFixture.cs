using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Queries;

namespace Pantry.Tests.Component.Unit.WebFeature.Queries;

[Trait("Category", "Unit")]
public class StorageLocationListQueryHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturn()
    {
        // Arrange
        var storageLocation1 = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 1, Name = "Test Location", Description = "Bar Description" };
        var storageLocation2 = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 2, Name = "Unit Location", Description = "Foo Description" };

        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.StorageLocations.Add(storageLocation1);
            dbContext.StorageLocations.Add(storageLocation2);
        });

        var queryHandler = new StorageLocationListQueryHandler(
            Substitute.For<ILogger<StorageLocationListQueryHandler>>(),
            testDatabase,
            PrincipalOfJohnDoeWithHousehold);

        // Act
        IReadOnlyCollection<StorageLocation> storageLocations = await queryHandler.ExecuteAsync(new StorageLocationListQuery());

        // Assert
        storageLocations.Count.ShouldBe(2);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmpty()
    {
        // Arrange
        var storageLocation1 = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 1, Name = "Test Location", Description = "Bar Description" };
        var storageLocation2 = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 2, Name = "Unit Location", Description = "Foo Description" };

        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.StorageLocations.Add(storageLocation1);
            dbContext.StorageLocations.Add(storageLocation2);
        });

        var queryHandler = new StorageLocationListQueryHandler(
            Substitute.For<ILogger<StorageLocationListQueryHandler>>(),
            testDatabase,
            PrincipalOfJohnDoe);

        // Act
        IReadOnlyCollection<StorageLocation> storageLocations = await queryHandler.ExecuteAsync(new StorageLocationListQuery());

        // Assert
        storageLocations.Count.ShouldBe(0);
    }
}
