using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Requests;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

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
        await testApplication.SetupDatabaseAsync<AppDbContext>(
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
            dbContext.Accounts.Count().ShouldBe(2);
            dbContext.Households.Count().ShouldBe(1);
            dbContext.Invitations.Count().ShouldBe(1);
            dbContext.Invitations.First().HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Invitations.First().CreatorId.ShouldBe(AccountJohnDoe.AccountId);
            dbContext.Invitations.First().FriendsCode.ShouldBe(AccountFooBar.FriendsCode);
            dbContext.Invitations.First().InvitationId.ShouldBe(1);
            dbContext.Invitations.First().ValidUntilDate.ShouldBe(contextDateTime.AddDays(10));
            dbContext.Accounts.First(x => x.AccountId == AccountJohnDoe.AccountId).HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.ShouldBeNull();
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
        await testApplication.SetupDatabaseAsync<AppDbContext>(
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
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(2);
            dbContext.Households.Count().ShouldBe(1);
            dbContext.Invitations.Count().ShouldBe(1);
            dbContext.Invitations.First().HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Invitations.First().CreatorId.ShouldBe(AccountJohnDoe.AccountId);
            dbContext.Invitations.First().FriendsCode.ShouldBe(AccountFooBar.FriendsCode);
            dbContext.Invitations.First().InvitationId.ShouldBe(1);
            dbContext.Invitations.First().ValidUntilDate.ShouldBe(validUntil);
            dbContext.Accounts.First(x => x.AccountId == AccountJohnDoe.AccountId).HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.ShouldBeNull();
        });
    }

    [Fact]
    public async Task GetInvitationAsync_ShouldWork()
    {
        var validUntil = DateTimeProvider.UtcNow;
        AccountJohnDoe.Household = HouseholdOfJohnDoe;
        var invitation = new Invitation { Creator = AccountJohnDoe, Household = HouseholdOfJohnDoe, FriendsCode = AccountFooBar.FriendsCode, ValidUntilDate = validUntil };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
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
        response.ShouldNotBeNull();
        response.Invitations?.Count().ShouldBe(1);
        response.Invitations!.First().HouseholdName.ShouldBe(HouseholdOfJohnDoe.Name);
        response.Invitations!.First().ValidUntilDate.ShouldBe(validUntil);
        response.Invitations!.First().CreatorName.ShouldBe($"{AccountJohnDoe.FirstName} {AccountJohnDoe.LastName}");
        response.Invitations!.First().FriendsCode.ShouldBe(AccountFooBar.FriendsCode);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(2);
            dbContext.Households.Count().ShouldBe(1);
            dbContext.Invitations.Count().ShouldBe(1);
            dbContext.Invitations.First().HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Invitations.First().CreatorId.ShouldBe(AccountJohnDoe.AccountId);
            dbContext.Invitations.First().FriendsCode.ShouldBe(AccountFooBar.FriendsCode);
            dbContext.Invitations.First().InvitationId.ShouldBe(1);
            dbContext.Invitations.First().ValidUntilDate.ShouldBe(validUntil);
            dbContext.Accounts.First(x => x.AccountId == AccountJohnDoe.AccountId).HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.ShouldBeNull();
        });
    }

    [Fact]
    public async Task PostAcceptInvitation_ShouldWork()
    {
        var validUntil = DateTimeProvider.UtcNow.AddDays(10);
        AccountFooBar.Household = HouseholdOfJohnDoe;
        var invitation = new Invitation { Creator = AccountFooBar, Household = HouseholdOfJohnDoe, FriendsCode = AccountJohnDoe.FriendsCode, ValidUntilDate = validUntil };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
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
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(2);
            dbContext.Households.Count().ShouldBe(1);
            dbContext.Invitations.Count().ShouldBe(0);
            dbContext.Accounts.First(x => x.AccountId == AccountJohnDoe.AccountId).HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
        });
    }

    [Fact]
    public async Task PostAcceptInvitationAsync_ShouldThrow()
    {
        var validUntil = DateTimeProvider.UtcNow.AddDays(-1);
        AccountFooBar.Household = HouseholdOfJohnDoe;
        var invitation = new Invitation { Creator = AccountFooBar, Household = HouseholdOfJohnDoe, FriendsCode = AccountJohnDoe.FriendsCode, ValidUntilDate = validUntil };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
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
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(2);
            dbContext.Households.Count().ShouldBe(1);
            dbContext.Invitations.Count().ShouldBe(0);
            dbContext.Accounts.First(x => x.AccountId == AccountJohnDoe.AccountId).HouseholdId.ShouldBeNull();
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
        });
    }

    [Fact]
    public async Task PostDeclineInvitation_ShouldWork()
    {
        var validUntil = DateTimeProvider.UtcNow.AddDays(10);
        AccountFooBar.Household = HouseholdOfJohnDoe;
        var invitation = new Invitation { Creator = AccountFooBar, Household = HouseholdOfJohnDoe, FriendsCode = AccountJohnDoe.FriendsCode, ValidUntilDate = validUntil };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
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
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(2);
            dbContext.Households.Count().ShouldBe(1);
            dbContext.Invitations.Count().ShouldBe(0);
            dbContext.Accounts.First(x => x.AccountId == AccountJohnDoe.AccountId).HouseholdId.ShouldBeNull();
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
        });
    }

    [Fact]
    public async Task PostDeclineInvitationAsync_IgnoreValidUntil_ShouldWork()
    {
        var validUntil = DateTimeProvider.UtcNow.AddDays(-1);
        AccountFooBar.Household = HouseholdOfJohnDoe;
        var invitation = new Invitation { Creator = AccountFooBar, Household = HouseholdOfJohnDoe, FriendsCode = AccountJohnDoe.FriendsCode, ValidUntilDate = validUntil };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
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
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(2);
            dbContext.Households.Count().ShouldBe(1);
            dbContext.Invitations.Count().ShouldBe(0);
            dbContext.Accounts.First(x => x.AccountId == AccountJohnDoe.AccountId).HouseholdId.ShouldBeNull();
            dbContext.Accounts.First(x => x.AccountId == AccountFooBar.AccountId).HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
        });
    }
}
