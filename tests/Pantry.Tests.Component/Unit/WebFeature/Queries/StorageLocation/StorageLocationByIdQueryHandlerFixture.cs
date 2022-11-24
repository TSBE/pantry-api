using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pantry.Common.EntityFrameworkCore.Exceptions;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Queries;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;

namespace Pantry.Tests.Component.Unit.WebFeature.Queries;

[Trait("Category", "Unit")]
public class StorageLocationByIdQueryHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturn()
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

        var queryHandler = new StorageLocationByIdQueryHandler(Substitute.For<ILogger<StorageLocationByIdQueryHandler>>(), testDatabase, PrincipalOfJohnDoeWithHousehold);

        // Act
        StorageLocation actual = await queryHandler.ExecuteAsync(new StorageLocationByIdQuery(storageLocation.StorageLocationId));

        // Assert
        actual.StorageLocationId.Should().Be(storageLocation.StorageLocationId);
        actual.Name.Should().Be(storageLocation.Name);
        actual.Description.Should().Be(storageLocation.Description);
        actual.HouseholdId.Should().Be(storageLocation.HouseholdId);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow()
    {
        // Arrange
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Households.Add(HouseholdOfJohnDoe);
        });

        var queryHandler = new StorageLocationByIdQueryHandler(Substitute.For<ILogger<StorageLocationByIdQueryHandler>>(), testDatabase, PrincipalOfJohnDoeWithHousehold);

        // Act
        Func<Task> act = async () => await queryHandler.ExecuteAsync(new StorageLocationByIdQuery(1));

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }
}
