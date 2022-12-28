using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Pantry.Common.Time;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Requests;
using Pantry.Features.WebFeature.V1.Controllers.Responses;
using Pantry.Tests.Common;
using Pantry.Tests.Component.Integration.Environment;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;
using Xunit.Abstractions;

namespace Pantry.Tests.Component.Integration.Controllers;

[Trait("Category", "Integration")]
public class InvitationControllerFixture : BaseControllerFixture
{
    public InvitationControllerFixture(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task PostInvitationAsync_ShouldReturnAccount()
    {
        // Arrange
        var contextDateTime = DateTimeProvider.UtcNow;

        AccountJohnDoe.Household = HouseholdOfJohnDoe;
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Accounts.Add(AccountFooBar);
            dbContext.Households.Add(HouseholdOfJohnDoe);
        });

        using HttpClient httpClient = testApplication.CreateClient();
        httpClient.StartDateTimeContext(contextDateTime);

        var expectedInvitationRequest = new InvitationRequest
        {
            FriendsCode = AccountFooBar.FriendsCode
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("api/v1/invitations", expectedInvitationRequest);

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Should().HaveCount(2);
            dbContext.Households.Should().HaveCount(1);
            dbContext.Invitations.Should().HaveCount(1);
            dbContext.Invitations.First().HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Invitations.First().CreatorId.Should().Be(AccountJohnDoe.AccountId);
            dbContext.Invitations.First().FriendsCode.Should().Be(AccountFooBar.FriendsCode);
            dbContext.Invitations.First().InvitationId.Should().Be(1);
            dbContext.Invitations.First().ValidUntilDate.Should().Be(contextDateTime.AddDays(10));
            dbContext.Accounts.First(x => x.AccountId == AccountJohnDoe.AccountId).HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.Should().BeNull();
        });
    }

    [Fact]
    public async Task PostInvitationAsync_ShouldThrow()
    {
        // Arrange
        var validUntil = DateTimeProvider.UtcNow;
        AccountJohnDoe.Household = HouseholdOfJohnDoe;
        var invitation = new Invitation { Creator = AccountJohnDoe, Household = HouseholdOfJohnDoe, FriendsCode = AccountFooBar.FriendsCode, ValidUntilDate = validUntil };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Accounts.Add(AccountFooBar);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.Invitations.Add(invitation);
        });

        using HttpClient httpClient = testApplication.CreateClient();

        var expectedInvitationRequest = new InvitationRequest
        {
            FriendsCode = AccountFooBar.FriendsCode
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("api/v1/invitations", expectedInvitationRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Should().HaveCount(2);
            dbContext.Households.Should().HaveCount(1);
            dbContext.Invitations.Should().HaveCount(1);
            dbContext.Invitations.First().HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Invitations.First().CreatorId.Should().Be(AccountJohnDoe.AccountId);
            dbContext.Invitations.First().FriendsCode.Should().Be(AccountFooBar.FriendsCode);
            dbContext.Invitations.First().InvitationId.Should().Be(1);
            dbContext.Invitations.First().ValidUntilDate.Should().Be(validUntil);
            dbContext.Accounts.First(x => x.AccountId == AccountJohnDoe.AccountId).HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.Should().BeNull();
        });
    }

    [Fact]
    public async Task GetInvitationAsync_ShouldWork()
    {
        var validUntil = DateTimeProvider.UtcNow;
        AccountJohnDoe.Household = HouseholdOfJohnDoe;
        var invitation = new Invitation { Creator = AccountJohnDoe, Household = HouseholdOfJohnDoe, FriendsCode = AccountFooBar.FriendsCode, ValidUntilDate = validUntil };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Accounts.Add(AccountFooBar);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.Invitations.Add(invitation);
        });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.GetFromJsonAsync<InvitationListResponse>("api/v1/invitations/my");

        // Assert
        response.Should().NotBeNull();
        response!.Invitations.Should().HaveCount(1);
        response!.Invitations!.First().HouseholdName.Should().Be(HouseholdOfJohnDoe.Name);
        response!.Invitations!.First().ValidUntilDate.Should().Be(validUntil);
        response!.Invitations!.First().CreatorName.Should().Be($"{AccountJohnDoe.FirstName} {AccountJohnDoe.LastName}");
        response!.Invitations!.First().FriendsCode.Should().Be(AccountFooBar.FriendsCode);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Should().HaveCount(2);
            dbContext.Households.Should().HaveCount(1);
            dbContext.Invitations.Should().HaveCount(1);
            dbContext.Invitations.First().HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Invitations.First().CreatorId.Should().Be(AccountJohnDoe.AccountId);
            dbContext.Invitations.First().FriendsCode.Should().Be(AccountFooBar.FriendsCode);
            dbContext.Invitations.First().InvitationId.Should().Be(1);
            dbContext.Invitations.First().ValidUntilDate.Should().Be(validUntil);
            dbContext.Accounts.First(x => x.AccountId == AccountJohnDoe.AccountId).HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.Should().BeNull();
        });
    }

    [Fact]
    public async Task PostAcceptInvitation_ShouldWork()
    {
        var validUntil = DateTimeProvider.UtcNow.AddDays(10);
        AccountFooBar.Household = HouseholdOfJohnDoe;
        var invitation = new Invitation { Creator = AccountFooBar, Household = HouseholdOfJohnDoe, FriendsCode = AccountJohnDoe.FriendsCode, ValidUntilDate = validUntil };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Accounts.Add(AccountFooBar);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.Invitations.Add(invitation);
        });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.PostAsync($"api/v1/invitations/{AccountJohnDoe.FriendsCode}/accept", null);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Should().HaveCount(2);
            dbContext.Households.Should().HaveCount(1);
            dbContext.Invitations.Should().HaveCount(0);
            dbContext.Accounts.First(x => x.AccountId == AccountJohnDoe.AccountId).HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
        });
    }

    [Fact]
    public async Task PostAcceptInvitationAsync_ShouldThrow()
    {
        var validUntil = DateTimeProvider.UtcNow.AddDays(-1);
        AccountFooBar.Household = HouseholdOfJohnDoe;
        var invitation = new Invitation { Creator = AccountFooBar, Household = HouseholdOfJohnDoe, FriendsCode = AccountJohnDoe.FriendsCode, ValidUntilDate = validUntil };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Accounts.Add(AccountFooBar);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.Invitations.Add(invitation);
        });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.PostAsync($"api/v1/invitations/{AccountJohnDoe.FriendsCode}/accept", null);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Should().HaveCount(2);
            dbContext.Households.Should().HaveCount(1);
            dbContext.Invitations.Should().HaveCount(0);
            dbContext.Accounts.First(x => x.AccountId == AccountJohnDoe.AccountId).HouseholdId.Should().BeNull();
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
        });
    }

    [Fact]
    public async Task PostDeclineInvitation_ShouldWork()
    {
        var validUntil = DateTimeProvider.UtcNow.AddDays(10);
        AccountFooBar.Household = HouseholdOfJohnDoe;
        var invitation = new Invitation { Creator = AccountFooBar, Household = HouseholdOfJohnDoe, FriendsCode = AccountJohnDoe.FriendsCode, ValidUntilDate = validUntil };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Accounts.Add(AccountFooBar);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.Invitations.Add(invitation);
        });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.PostAsync($"api/v1/invitations/{AccountJohnDoe.FriendsCode}/decline", null);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Should().HaveCount(2);
            dbContext.Households.Should().HaveCount(1);
            dbContext.Invitations.Should().HaveCount(0);
            dbContext.Accounts.First(x => x.AccountId == AccountJohnDoe.AccountId).HouseholdId.Should().BeNull();
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
        });
    }

    [Fact]
    public async Task PostDeclineInvitationAsync_IgnoreValidUntil_ShouldWork()
    {
        var validUntil = DateTimeProvider.UtcNow.AddDays(-1);
        AccountFooBar.Household = HouseholdOfJohnDoe;
        var invitation = new Invitation { Creator = AccountFooBar, Household = HouseholdOfJohnDoe, FriendsCode = AccountJohnDoe.FriendsCode, ValidUntilDate = validUntil };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Accounts.Add(AccountFooBar);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.Invitations.Add(invitation);
        });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.PostAsync($"api/v1/invitations/{AccountJohnDoe.FriendsCode}/decline", null);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Should().HaveCount(2);
            dbContext.Households.Should().HaveCount(1);
            dbContext.Invitations.Should().HaveCount(0);
            dbContext.Accounts.First(x => x.AccountId == AccountJohnDoe.AccountId).HouseholdId.Should().BeNull();
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
        });
    }
}
