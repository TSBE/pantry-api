using Pantry.Core.Persistence;
using Pantry.Features.WebFeature.V1.Controllers.Requests;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Tests.Component.Integration.Controllers;

[Trait("Category", "Integration")]
public class AccountControllerFixture : BaseControllerFixture
{
    public AccountControllerFixture(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task PutAccountAsync_ShouldReturnAccount()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);

        using HttpClient httpClient = testApplication.CreateClient();

        var expectedAccountRequest = new AccountRequest
        {
            FirstName = "Jane",
            LastName = "Doe",
        };

        // Act
        var response = await httpClient.PutAsJsonAsync("api/v1/accounts/me", expectedAccountRequest);

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(1);
            dbContext.Accounts.FirstOrDefault()!.FirstName.ShouldBe(expectedAccountRequest.FirstName);
            dbContext.Accounts.FirstOrDefault()!.LastName.ShouldBe(expectedAccountRequest.LastName);
            dbContext.Accounts.FirstOrDefault()!.FriendsCode.ShouldNotBe(Guid.Empty);
            dbContext.Accounts.FirstOrDefault()!.OAuhtId.ShouldBe(PrincipalJohnDoeId);
        });
    }

    [Fact]
    public async Task PutAccountAsync_ShouldReturnUpdatedAccount()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        var expectedAccountRequest = new AccountRequest
        {
            FirstName = "Jane",
            LastName = "Doe",
        };

        // Act
        var response = await httpClient.PutAsJsonAsync("api/v1/accounts/me", expectedAccountRequest);

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(1);
            dbContext.Accounts.FirstOrDefault()!.FirstName.ShouldBe(expectedAccountRequest.FirstName);
            dbContext.Accounts.FirstOrDefault()!.LastName.ShouldBe(expectedAccountRequest.LastName);
            dbContext.Accounts.FirstOrDefault()!.FriendsCode.ShouldBe(AccountJohnDoe.FriendsCode);
            dbContext.Accounts.FirstOrDefault()!.OAuhtId.ShouldBe(AccountJohnDoe.OAuhtId);
        });
    }

    [Fact]
    public async Task GetAccountAsync_ShouldWork()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.GetFromJsonAsync<AccountResponse>("api/v1/accounts/me");

        // Assert
        response.ShouldNotBeNull();
        response.FirstName.ShouldBe(AccountJohnDoe.FirstName);
        response.LastName.ShouldBe(AccountJohnDoe.LastName);
        response.FriendsCode.ShouldBe(AccountJohnDoe.FriendsCode);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(1);
        });
    }

    [Fact]
    public async Task GetAccountAsync_ShouldThrow()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>();

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.GetAsync("api/v1/accounts/me");

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(0);
        });
    }

    [Fact]
    public async Task DeleteAccountAsync_ShouldWork()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.DeleteAsync("api/v1/accounts/me");

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(0);
        });
    }

    [Fact]
    public async Task DeleteAccountAsync_ShouldThrow()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>();

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.DeleteAsync("api/v1/accounts/me");

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(0);
        });
    }
}
