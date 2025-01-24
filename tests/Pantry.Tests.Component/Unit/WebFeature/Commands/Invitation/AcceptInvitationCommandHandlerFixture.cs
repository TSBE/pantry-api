using Microsoft.Extensions.Logging;
using Pantry.Common.EntityFrameworkCore.Exceptions;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Commands;

namespace Pantry.Tests.Component.Unit.WebFeature.Commands;

[Trait("Category", "Unit")]
public class AcceptInvitationCommandHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldAcceptInvitation()
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
        var commandHandler = new AcceptInvitationCommandHandler(
            Substitute.For<ILogger<AcceptInvitationCommandHandler>>(),
            testDatabase,
            PrincipalOfFooBar);

        // Act
        await commandHandler.ExecuteAsync(new AcceptInvitationCommand(AccountFooBar.FriendsCode));

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

    [Fact]
    public async Task ExecuteAsync_ShouldFailTooLateAccept()
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
        var commandHandler = new AcceptInvitationCommandHandler(
            Substitute.For<ILogger<AcceptInvitationCommandHandler>>(),
            testDatabase,
            PrincipalOfFooBar);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new AcceptInvitationCommand(AccountFooBar.FriendsCode));

        // Assert
        await act.ShouldThrowAsync<Opw.HttpExceptions.BadRequestException>();
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
        var commandHandler = new AcceptInvitationCommandHandler(
            Substitute.For<ILogger<AcceptInvitationCommandHandler>>(),
            testDatabase,
            PrincipalOfFooBar);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new AcceptInvitationCommand(AccountFooBar.FriendsCode));

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
        var commandHandler = new AcceptInvitationCommandHandler(
            Substitute.For<ILogger<AcceptInvitationCommandHandler>>(),
            testDatabase,
            PrincipalOfFooBar);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new AcceptInvitationCommand(Guid.NewGuid()));

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
    public async Task ExecuteAsync_HasHousehold_ShouldThrow()
    {
        // Arrange
        AccountJohnDoe.Household = HouseholdOfJohnDoe;
        AccountFooBar.Household = HouseholdOfJohnDoe;
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Accounts.Add(AccountFooBar);
            dbContext.Households.Add(HouseholdOfJohnDoe);
        });
        var commandHandler = new AcceptInvitationCommandHandler(
            Substitute.For<ILogger<AcceptInvitationCommandHandler>>(),
            testDatabase,
            PrincipalOfFooBarWithJohnDoesHousehold);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new AcceptInvitationCommand(AccountFooBar.FriendsCode));

        // Assert
        await act.ShouldThrowAsync<Opw.HttpExceptions.ForbiddenException>();
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
