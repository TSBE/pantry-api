using Microsoft.Extensions.Logging;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Commands;

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
        act.FirstName.ShouldBeEquivalentTo("Jane");
        act.LastName.ShouldBeEquivalentTo("Doe");
        act.OAuhtId.ShouldBeEquivalentTo(PrincipalJohnDoeId);
        act.FriendsCode.ShouldNotBe(Guid.Empty);
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
        act.FirstName.ShouldBeEquivalentTo("John");
        act.LastName.ShouldBeEquivalentTo("Smith");
        act.OAuhtId.ShouldBeEquivalentTo(PrincipalJohnDoeId);
        act.FriendsCode.ShouldBe(account.FriendsCode);
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
        await act.ShouldThrowAsync<Opw.HttpExceptions.ForbiddenException>();
    }
}
