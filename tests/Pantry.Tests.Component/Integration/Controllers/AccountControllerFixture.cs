using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Pantry.Core.Persistence;
using Pantry.Features.WebFeature.V1.Controllers.Requests;
using Pantry.Features.WebFeature.V1.Controllers.Responses;
using Pantry.Tests.Component.Integration.Environment;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Xunit;
using Xunit.Abstractions;

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
            dbContext.Accounts.Count().Should().Be(1);
            dbContext.Accounts.FirstOrDefault()!.FirstName.Should().Be(expectedAccountRequest.FirstName);
            dbContext.Accounts.FirstOrDefault()!.LastName.Should().Be(expectedAccountRequest.LastName);
            dbContext.Accounts.FirstOrDefault()!.FriendsCode.Should().NotBeEmpty();
            dbContext.Accounts.FirstOrDefault()!.OAuhtId.Should().Be(PrincipalJohnDoeId);
        });
    }

    [Fact]
    public async Task PutAccountAsync_ShouldReturnUpdatedAccount()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
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
            dbContext.Accounts.Count().Should().Be(1);
            dbContext.Accounts.FirstOrDefault()!.FirstName.Should().Be(expectedAccountRequest.FirstName);
            dbContext.Accounts.FirstOrDefault()!.LastName.Should().Be(expectedAccountRequest.LastName);
            dbContext.Accounts.FirstOrDefault()!.FriendsCode.Should().Be(AccountJohnDoe.FriendsCode);
            dbContext.Accounts.FirstOrDefault()!.OAuhtId.Should().Be(AccountJohnDoe.OAuhtId);
        });
    }

    [Fact]
    public async Task GetAccountAsync_ShouldWork()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.GetFromJsonAsync<AccountResponse>("api/v1/accounts/me");

        // Assert
        response.Should().NotBeNull();
        response!.FirstName.Should().Be(AccountJohnDoe.FirstName);
        response!.LastName.Should().Be(AccountJohnDoe.LastName);
        response!.FriendsCode.Should().Be(AccountJohnDoe.FriendsCode);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().Should().Be(1);
        });
    }

    [Fact]
    public async Task GetAccountAsync_ShouldThrow()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>();

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.GetAsync("api/v1/accounts/me");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().Should().Be(0);
        });
    }

    [Fact]
    public async Task DeleteAccountAsync_ShouldWork()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
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
            dbContext.Accounts.Count().Should().Be(0);
        });
    }

    [Fact]
    public async Task DeleteAccountAsync_ShouldThrow()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>();

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.DeleteAsync("api/v1/accounts/me");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().Should().Be(0);
        });
    }
}
