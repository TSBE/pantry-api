using System;
using System.Security.Cryptography.X509Certificates;
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
        act.Name.Should().Be("Updated Location");
        act.Description.Should().Be("Updated Description");
        act.HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
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
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }
}
