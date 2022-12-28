using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Requests;
using Pantry.Features.WebFeature.V1.Controllers.Responses;
using Pantry.Tests.Component.Integration.Environment;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Pantry.Tests.Component.Integration.Controllers;

[Trait("Category", "Integration")]
public class HouseholdControllerFixture : BaseControllerFixture
{
    public HouseholdControllerFixture(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task PostHouseholdAsync_ShouldReturnAccount()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        var expectedHouseholdRequest = new HouseholdRequest
        {
            Name = "Test",
            SubscriptionType = Features.WebFeature.V1.Controllers.Enums.SubscriptionType.FREE,
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("api/v1/households/my", expectedHouseholdRequest);

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Households.Count().Should().Be(1);
            dbContext.Households.FirstOrDefault()!.Name.Should().Be(expectedHouseholdRequest.Name);
            dbContext.Households.FirstOrDefault()!.SubscriptionType.Should().Be(Core.Persistence.Enums.SubscriptionType.FREE);
        });
    }

    [Fact]
    public async Task PostHouseholdAsync_ShouldThrow()
    {
        // Arrange
        var household = new Household { Name = "Test", SubscriptionType = Core.Persistence.Enums.SubscriptionType.FREE };
        AccountJohnDoe.Household = household;
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Households.Add(household);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        var expectedHouseholdRequest = new HouseholdRequest
        {
            Name = "Test",
            SubscriptionType = Features.WebFeature.V1.Controllers.Enums.SubscriptionType.FREE,
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("api/v1/households/my", expectedHouseholdRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Households.Count().Should().Be(1);
            dbContext.Households.FirstOrDefault()!.Name.Should().Be(expectedHouseholdRequest.Name);
            dbContext.Households.FirstOrDefault()!.SubscriptionType.Should().Be(Core.Persistence.Enums.SubscriptionType.FREE);
        });
    }

    [Fact]
    public async Task GetHouseholdAsync_ShouldWork()
    {
        // Arrange
        var household = new Household { Name = "Test", SubscriptionType = Core.Persistence.Enums.SubscriptionType.FREE };
        AccountJohnDoe.Household = household;
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Households.Add(household);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.GetFromJsonAsync<HouseholdResponse>("api/v1/households/my", JsonSerializerOptions);

        // Assert
        response.Should().NotBeNull();
        response!.Name.Should().Be("Test");
        response!.SubscriptionType.Should().Be(Features.WebFeature.V1.Controllers.Enums.SubscriptionType.FREE);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().Should().Be(1);
            dbContext.Households.Count().Should().Be(1);
        });
    }

    [Fact]
    public async Task GetHouseholdAsync_ShouldThrow()
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
        var response = await httpClient.GetAsync("api/v1/households/my");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().Should().Be(1);
            dbContext.Households.Count().Should().Be(0);
        });
    }
}
