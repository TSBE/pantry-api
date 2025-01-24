using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Tests.Component.Integration.Controllers;

[Trait("Category", "Integration")]
public class MetadataControllerFixture : BaseControllerFixture
{
    public MetadataControllerFixture(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task GetMetadataAsync_ShouldReturnFoodFacts()
    {
        // Arrange
        var metadata = new Metadata { GlobalTradeItemNumber = "GTIN", FoodFacts = new Core.Models.OpenFoodFacts.Product { ProductName = "Unittest" } };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Households.Add(HouseholdOfJohnDoe);
                dbContext.Metadatas.Add(metadata);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.GetFromJsonAsync<MetadataResponse>($"api/v1/metadatas/{metadata.GlobalTradeItemNumber}", JsonSerializerOptions);

        // Assert
        response!.Name.ShouldBe(metadata.FoodFacts.ProductName);
        response!.GlobalTradeItemNumber.ShouldBe(metadata.GlobalTradeItemNumber);
    }

    [Fact]
    public async Task GetMetadataAsync_ShouldReturnNonFood()
    {
        // Arrange
        var metadata = new Metadata { GlobalTradeItemNumber = "GTIN", ProductFacts = new Core.Models.EanSearchOrg.NonFoodProduct { Name = "Unittest" } };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Households.Add(HouseholdOfJohnDoe);
                dbContext.Metadatas.Add(metadata);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.GetFromJsonAsync<MetadataResponse>($"api/v1/metadatas/{metadata.GlobalTradeItemNumber}", JsonSerializerOptions);

        // Assert
        response!.Name.ShouldBe(metadata.ProductFacts.Name);
        response!.GlobalTradeItemNumber.ShouldBe(metadata.GlobalTradeItemNumber);
    }
}
