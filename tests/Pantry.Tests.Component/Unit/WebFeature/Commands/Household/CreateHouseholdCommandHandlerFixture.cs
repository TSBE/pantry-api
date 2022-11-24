using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Core.Persistence.Enums;
using Pantry.Features.WebFeature.Commands;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;

namespace Pantry.Tests.Component.Unit.WebFeature.Commands;

[Trait("Category", "Unit")]
public class CreateHouseholdCommandHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldCreateHousehold()
    {
        // Arrange
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
        });
        var commandHandler = new CreateHouseholdCommandHandler(
            Substitute.For<ILogger<CreateHouseholdCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoe);

        // Act
        var act = await commandHandler.ExecuteAsync(new CreateHouseholdCommand("Test", SubscriptionType.FREE));

        // Assert
        act.Name.Should().Be("Test");
        act.SubscriptionType.Should().Be(SubscriptionType.FREE);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotCreateHousehold()
    {
        // Arrange
        var household = new Household { Name = "Test", SubscriptionType = SubscriptionType.FREE };
        AccountJohnDoe.Household = household;
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Households.Add(household);
        });
        var commandHandler = new CreateHouseholdCommandHandler(
            Substitute.For<ILogger<CreateHouseholdCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoe);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new CreateHouseholdCommand("Test", SubscriptionType.FREE));

        // Assert
        await act.Should().ThrowAsync<Opw.HttpExceptions.BadRequestException>();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow()
    {
        // Arrange
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase();
        var commandHandler = new CreateHouseholdCommandHandler(
            Substitute.For<ILogger<CreateHouseholdCommandHandler>>(),
            testDatabase,
            PrincipalEmpty);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new CreateHouseholdCommand("Test", SubscriptionType.FREE));

        // Assert
        await act.Should().ThrowAsync<Opw.HttpExceptions.ForbiddenException>();
    }
}
