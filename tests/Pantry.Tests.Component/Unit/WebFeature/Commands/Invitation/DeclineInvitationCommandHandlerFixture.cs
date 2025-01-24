using Microsoft.Extensions.Logging;
using Pantry.Common.EntityFrameworkCore.Exceptions;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Commands;

namespace Pantry.Tests.Component.Unit.WebFeature.Commands;

[Trait("Category", "Unit")]
public class DeclineInvitationCommandHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldDeclineInvitation()
    {
        // Arrange
        var validUntil = DateTimeProvider.UtcNow;
        using var context = new DateTimeContext(validUntil);
        var invitation = new Invitation { Creator = AccountJohnDoe, Household = HouseholdOfJohnDoe, FriendsCode = AccountFooBar.FriendsCode, ValidUntilDate = validUntil.AddDays(10) };
        AccountJohnDoe.Household = HouseholdOfJohnDoe;
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Accounts.Add(AccountFooBar);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.Invitations.Add(invitation);
        });
        var commandHandler = new DeclineInvitationCommandHandler(
            Substitute.For<ILogger<DeclineInvitationCommandHandler>>(),
            testDatabase,
            PrincipalOfFooBar);

        // Act
        await commandHandler.ExecuteAsync(new DeclineInvitationCommand(AccountFooBar.FriendsCode));

        // Assert
        testDatabase.AssertDatabaseContent(
            dbContext =>
            {
                dbContext.Accounts.Count().ShouldBe(2);
                dbContext.Households.Count().ShouldBe(1);
                dbContext.Invitations.Count().ShouldBe(0);
                dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.ShouldBeNull();
            });
    }

    [Fact]
    public async Task ExecuteAsync_TooLate_ShouldWork()
    {
        // Arrange
        var validUntil = DateTimeProvider.UtcNow;
        using var context = new DateTimeContext(validUntil);
        var invitation = new Invitation { Creator = AccountJohnDoe, Household = HouseholdOfJohnDoe, FriendsCode = AccountFooBar.FriendsCode, ValidUntilDate = validUntil.AddDays(-1) };
        AccountJohnDoe.Household = HouseholdOfJohnDoe;
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Accounts.Add(AccountFooBar);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.Invitations.Add(invitation);
        });
        var commandHandler = new DeclineInvitationCommandHandler(
            Substitute.For<ILogger<DeclineInvitationCommandHandler>>(),
            testDatabase,
            PrincipalOfFooBar);

        // Act
        await commandHandler.ExecuteAsync(new DeclineInvitationCommand(AccountFooBar.FriendsCode));

        // Assert
        testDatabase.AssertDatabaseContent(
        dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(2);
            dbContext.Households.Count().ShouldBe(1);
            dbContext.Invitations.Count().ShouldBe(0);
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.ShouldBeNull();
        });
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotFound()
    {
        // Arrange
        var validUntil = DateTimeProvider.UtcNow;
        using var context = new DateTimeContext(validUntil);
        AccountJohnDoe.Household = HouseholdOfJohnDoe;
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Accounts.Add(AccountFooBar);
            dbContext.Households.Add(HouseholdOfJohnDoe);
        });
        var commandHandler = new DeclineInvitationCommandHandler(
            Substitute.For<ILogger<DeclineInvitationCommandHandler>>(),
            testDatabase,
            PrincipalOfFooBar);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new DeclineInvitationCommand(AccountFooBar.FriendsCode));

        // Assert
        await act.ShouldThrowAsync<EntityNotFoundException<Invitation>>();
        testDatabase.AssertDatabaseContent(
        dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(2);
            dbContext.Households.Count().ShouldBe(1);
            dbContext.Invitations.Count().ShouldBe(0);
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.ShouldBeNull();
        });
    }

    [Fact]
    public async Task ExecuteAsync_WrongUser_ShouldThrow()
    {
        // Arrange
        var validUntil = DateTimeProvider.UtcNow;
        using var context = new DateTimeContext(validUntil);
        AccountJohnDoe.Household = HouseholdOfJohnDoe;
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Accounts.Add(AccountFooBar);
            dbContext.Households.Add(HouseholdOfJohnDoe);
        });
        var commandHandler = new DeclineInvitationCommandHandler(
            Substitute.For<ILogger<DeclineInvitationCommandHandler>>(),
            testDatabase,
            PrincipalOfFooBar);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new DeclineInvitationCommand(Guid.NewGuid()));

        // Assert
        await act.ShouldThrowAsync<Opw.HttpExceptions.ForbiddenException>();
        testDatabase.AssertDatabaseContent(
        dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(2);
            dbContext.Households.Count().ShouldBe(1);
            dbContext.Invitations.Count().ShouldBe(0);
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.ShouldBeNull();
        });
    }

    [Fact]
    public async Task ExecuteAsync_HasHousehold_ShouldWork()
    {
        // Arrange
        AccountJohnDoe.Household = HouseholdOfJohnDoe;
        AccountFooBar.Household = HouseholdOfJohnDoe;
        var invitation = new Invitation { Creator = AccountJohnDoe, Household = HouseholdOfJohnDoe, FriendsCode = AccountFooBar.FriendsCode, ValidUntilDate = DateTimeProvider.UtcNow };
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Accounts.Add(AccountFooBar);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.Invitations.Add(invitation);
        });
        var commandHandler = new DeclineInvitationCommandHandler(
            Substitute.For<ILogger<DeclineInvitationCommandHandler>>(),
            testDatabase,
            PrincipalOfFooBarWithJohnDoesHousehold);

        // Act
        await commandHandler.ExecuteAsync(new DeclineInvitationCommand(AccountFooBar.FriendsCode));

        // Assert
        testDatabase.AssertDatabaseContent(
        dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(2);
            dbContext.Households.Count().ShouldBe(1);
            dbContext.Invitations.Count().ShouldBe(0);
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
        });
    }
}
