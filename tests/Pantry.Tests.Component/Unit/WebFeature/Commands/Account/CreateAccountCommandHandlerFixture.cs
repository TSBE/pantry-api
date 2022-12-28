using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Commands;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;

namespace Pantry.Tests.Component.Unit.WebFeature.Commands;

[Trait("Category", "Unit")]
public class CreateAccountCommandHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldCreateAccount()
    {
        // Arrange
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase();
        var commandHandler = new CreateAccountCommandHandler(
            Substitute.For<ILogger<CreateAccountCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoe);

        // Act
        var act = await commandHandler.ExecuteAsync(new CreateAccountCommand("Jane", "Doe"));

        // Assert
        act.FirstName.Should().BeEquivalentTo("Jane");
        act.LastName.Should().BeEquivalentTo("Doe");
        act.OAuhtId.Should().BeEquivalentTo(PrincipalJohnDoeId);
        act.FriendsCode.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldUpdateAccount()
    {
        // Arrange
        var account = new Account { AccountId = 1, FirstName = "Jane", LastName = "Doe", FriendsCode = Guid.NewGuid(), OAuhtId = PrincipalJohnDoeId };
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(account);
        });
        var commandHandler = new CreateAccountCommandHandler(
            Substitute.For<ILogger<CreateAccountCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoe);

        // Act
        var act = await commandHandler.ExecuteAsync(new CreateAccountCommand("John", "Smith"));

        // Assert
        act.FirstName.Should().BeEquivalentTo("John");
        act.LastName.Should().BeEquivalentTo("Smith");
        act.OAuhtId.Should().BeEquivalentTo(PrincipalJohnDoeId);
        act.FriendsCode.Should().Be(account.FriendsCode);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow()
    {
        // Arrange
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase();
        var commandHandler = new CreateAccountCommandHandler(
            Substitute.For<ILogger<CreateAccountCommandHandler>>(),
            testDatabase,
            PrincipalEmpty);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new CreateAccountCommand("John", "Doe"));

        // Assert
        await act.Should().ThrowAsync<Opw.HttpExceptions.ForbiddenException>();
    }
}
