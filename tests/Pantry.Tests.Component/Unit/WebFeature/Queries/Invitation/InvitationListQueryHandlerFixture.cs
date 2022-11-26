using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pantry.Common.Time;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Queries;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;

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
        actual.Should().HaveCount(1);
        actual.First().CreatorId.Should().Be(AccountJohnDoe.AccountId);
        actual.First().HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
        actual.First().FriendsCode.Should().Be(AccountFooBar.FriendsCode);
        actual.First().ValidUntilDate.Should().Be(invitation.ValidUntilDate);
        actual.First().Household.Should().NotBeNull();
        actual.First().Creator.Should().NotBeNull();
        testDatabase.AssertDatabaseContent(
        dbContext =>
        {
            dbContext.Invitations.Should().HaveCount(1);
            dbContext.Households.Should().HaveCount(1);
            dbContext.Accounts.Should().HaveCount(1);
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
        actual.Should().HaveCount(1);
        actual.First().CreatorId.Should().Be(AccountJohnDoe.AccountId);
        actual.First().HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
        actual.First().FriendsCode.Should().Be(AccountFooBar.FriendsCode);
        actual.First().ValidUntilDate.Should().Be(invitation.ValidUntilDate);
        testDatabase.AssertDatabaseContent(
        dbContext =>
        {
            dbContext.Invitations.Should().HaveCount(1);
            dbContext.Households.Should().HaveCount(1);
            dbContext.Accounts.Should().HaveCount(2);
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
        actual.Should().HaveCount(0);
        testDatabase.AssertDatabaseContent(
        dbContext =>
        {
            dbContext.Invitations.Should().HaveCount(1);
            dbContext.Households.Should().HaveCount(1);
            dbContext.Accounts.Should().HaveCount(3);
        });
    }
}
