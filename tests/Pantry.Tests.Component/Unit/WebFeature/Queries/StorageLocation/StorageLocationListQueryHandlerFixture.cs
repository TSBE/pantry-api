using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Queries;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;

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
        storageLocations.Should().HaveCount(2);
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
        storageLocations.Should().HaveCount(0);
    }
}
