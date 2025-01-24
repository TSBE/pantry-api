using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Queries;

namespace Pantry.Tests.Component.Unit.WebFeature.Queries;

[Trait("Category", "Unit")]
public class InvitationListQueryHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnForCreator()
    {
        // Arrange
        var invitation = new Invitation { Creator = AccountJohnDoe, Household = HouseholdOfJohnDoe, FriendsCode = AccountFooBar.FriendsCode, ValidUntilDate = DateTimeProvider.UtcNow };
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.Invitations.Add(invitation);
        });

        var queryHandler = new InvitationListQueryHandler(Substitute.For<ILogger<InvitationListQueryHandler>>(), testDatabase, PrincipalOfJohnDoeWithHousehold);

        // Act
        IReadOnlyCollection<Invitation> actual = await queryHandler.ExecuteAsync(new InvitationListQuery());

        // Assert
        actual.Count.ShouldBe(1);
        actual.First().CreatorId.ShouldBe(AccountJohnDoe.AccountId);
        actual.First().HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
        actual.First().FriendsCode.ShouldBe(AccountFooBar.FriendsCode);
        actual.First().ValidUntilDate.ShouldBe(invitation.ValidUntilDate);
        actual.First().Household.ShouldNotBeNull();
        actual.First().Creator.ShouldNotBeNull();
        testDatabase.AssertDatabaseContent(
        dbContext =>
        {
            dbContext.Invitations.Count().ShouldBe(1);
            dbContext.Households.Count().ShouldBe(1);
            dbContext.Accounts.Count().ShouldBe(1);
        });
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnForFriend()
    {
        // Arrange
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

        var queryHandler = new InvitationListQueryHandler(Substitute.For<ILogger<InvitationListQueryHandler>>(), testDatabase, PrincipalOfFooBar);

        // Act
        IReadOnlyCollection<Invitation> actual = await queryHandler.ExecuteAsync(new InvitationListQuery());

        // Assert
        actual.Count.ShouldBe(1);
        actual.First().CreatorId.ShouldBe(AccountJohnDoe.AccountId);
        actual.First().HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
        actual.First().FriendsCode.ShouldBe(AccountFooBar.FriendsCode);
        actual.First().ValidUntilDate.ShouldBe(invitation.ValidUntilDate);
        testDatabase.AssertDatabaseContent(
        dbContext =>
        {
            dbContext.Invitations.Count().ShouldBe(1);
            dbContext.Households.Count().ShouldBe(1);
            dbContext.Accounts.Count().ShouldBe(2);
        });
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmpty()
    {
        // Arrange
        var account = new Account { AccountId = 3, FirstName = "Jane", LastName = "Doe", FriendsCode = Guid.NewGuid(), OAuhtId = "AnyId" };

        var invitation = new Invitation { Creator = AccountJohnDoe, Household = HouseholdOfJohnDoe, FriendsCode = account.FriendsCode, ValidUntilDate = DateTimeProvider.UtcNow };
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Accounts.Add(AccountFooBar);
            dbContext.Accounts.Add(account);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.Invitations.Add(invitation);
        });

        var queryHandler = new InvitationListQueryHandler(Substitute.For<ILogger<InvitationListQueryHandler>>(), testDatabase, PrincipalOfFooBar);

        // Act
        IReadOnlyCollection<Invitation> actual = await queryHandler.ExecuteAsync(new InvitationListQuery());

        // Assert
        actual.Count.ShouldBe(0);
        testDatabase.AssertDatabaseContent(
        dbContext =>
        {
            dbContext.Invitations.Count().ShouldBe(1);
            dbContext.Households.Count().ShouldBe(1);
            dbContext.Accounts.Count().ShouldBe(3);
        });
    }
}
