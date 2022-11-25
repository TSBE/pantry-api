using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pantry.Common.Time;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Commands;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;

namespace Pantry.Tests.Component.Unit.WebFeature.Commands;

[Trait("Category", "Unit")]
public class CreateInvitationCommandHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldCreateInvitation()
    {
        // Arrange
        AccountJohnDoe.Household = HouseholdOfJohnDoe;
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Accounts.Add(AccountFooBar);
            dbContext.Households.Add(HouseholdOfJohnDoe);
        });
        var commandHandler = new CreateInvitationCommandHandler(
            Substitute.For<ILogger<CreateInvitationCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoeWithHousehold);

        var validUntil = DateTimeProvider.UtcNow;
        using var context = new DateTimeContext(validUntil);

        // Act
        var act = await commandHandler.ExecuteAsync(new CreateInvitationCommand(AccountFooBar.FriendsCode));

        // Assert
        act.InvitationId.Should().Be(1);
        act.FriendsCode.Should().Be(AccountFooBar.FriendsCode);
        act.CreatorId.Should().Be(AccountJohnDoe.AccountId);
        act.HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
        act.ValidUntilDate.Should().Be(validUntil.AddDays(10));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotCreateInvitation()
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
        var commandHandler = new CreateInvitationCommandHandler(
            Substitute.For<ILogger<CreateInvitationCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoeWithHousehold);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new CreateInvitationCommand(AccountFooBar.FriendsCode));

        // Assert
        await act.Should().ThrowAsync<Opw.HttpExceptions.BadRequestException>();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrowNotOwner()
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
        var commandHandler = new CreateInvitationCommandHandler(
            Substitute.For<ILogger<CreateInvitationCommandHandler>>(),
            testDatabase,
            PrincipalOfFooBarWithJohnDoesHousehold);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new CreateInvitationCommand(AccountFooBar.FriendsCode));

        // Assert
        await act.Should().ThrowAsync<Opw.HttpExceptions.ForbiddenException>();
    }
}
