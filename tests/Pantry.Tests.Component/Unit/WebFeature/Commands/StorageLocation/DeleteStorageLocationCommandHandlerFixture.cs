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
public class DeleteStorageLocationCommandHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldDeleteStorageLocation()
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
        var commandHandler = new DeleteStorageLocationCommandHandler(
            Substitute.For<ILogger<DeleteStorageLocationCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoeWithHousehold);

        // Act
        await commandHandler.ExecuteAsync(new DeleteStorageLocationCommand(storageLocation1.StorageLocationId));

        // Assert
        testDatabase.AssertDatabaseContent(
            dbContext =>
            {
                dbContext.Accounts.Should().HaveCount(1);
                dbContext.Households.Should().HaveCount(1);
                dbContext.StorageLocations.Should().HaveCount(1);
            });
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
        var commandHandler = new DeleteStorageLocationCommandHandler(
            Substitute.For<ILogger<DeleteStorageLocationCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoeWithHousehold);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new DeleteStorageLocationCommand(1));

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }
}
