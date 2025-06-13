using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Requests;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

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
        await testApplication.SetupDatabaseAsync<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        var expectedHouseholdRequest = new HouseholdRequest
        {
            Name = "Test",
            SubscriptionType = Features.WebFeature.V1.Controllers.Enums.SubscriptionTypeDto.FREE,
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("api/v1/households/my", expectedHouseholdRequest);

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Households.Count().ShouldBe(1);
            dbContext.Households.FirstOrDefault()!.Name.ShouldBe(expectedHouseholdRequest.Name);
            dbContext.Households.FirstOrDefault()!.SubscriptionType.ShouldBe(Core.Persistence.Enums.SubscriptionType.FREE);
        });
    }

    [Fact]
    public async Task PostHouseholdAsync_ShouldThrow()
    {
        // Arrange
        var household = new Household { Name = "Test", SubscriptionType = Core.Persistence.Enums.SubscriptionType.FREE };
        AccountJohnDoe.Household = household;
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Households.Add(household);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        var expectedHouseholdRequest = new HouseholdRequest
        {
            Name = "Test",
            SubscriptionType = Features.WebFeature.V1.Controllers.Enums.SubscriptionTypeDto.FREE,
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("api/v1/households/my", expectedHouseholdRequest);

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Households.Count().ShouldBe(1);
            dbContext.Households.FirstOrDefault()!.Name.ShouldBe(expectedHouseholdRequest.Name);
            dbContext.Households.FirstOrDefault()!.SubscriptionType.ShouldBe(Core.Persistence.Enums.SubscriptionType.FREE);
        });
    }

    [Fact]
    public async Task GetHouseholdAsync_ShouldWork()
    {
        // Arrange
        var household = new Household { Name = "Test", SubscriptionType = Core.Persistence.Enums.SubscriptionType.FREE };
        AccountJohnDoe.Household = household;
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Households.Add(household);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.GetFromJsonAsync<HouseholdResponse>("api/v1/households/my", JsonSerializerOptions);

        // Assert
        response.ShouldNotBeNull();
        response.Name.ShouldBe("Test");
        response.SubscriptionType.ShouldBe(Features.WebFeature.V1.Controllers.Enums.SubscriptionTypeDto.FREE);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(1);
            dbContext.Households.Count().ShouldBe(1);
        });
    }

    [Fact]
    public async Task GetHouseholdAsync_ShouldThrow()
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
        var response = await httpClient.GetAsync("api/v1/households/my");

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Accounts.Count().ShouldBe(1);
            dbContext.Households.Count().ShouldBe(0);
        });
    }
}
