using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Enums;
using Pantry.Features.WebFeature.V1.Controllers.Requests;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

namespace Pantry.Tests.Component.Integration.Controllers;

[Trait("Category", "Integration")]
public class ArticleControllerFixture : BaseControllerFixture
{
    public ArticleControllerFixture(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task GetArticleListAsync_ShouldReturnArticles()
    {
        // Arrange
        var article1 = new Article
        {
            Household = HouseholdOfJohnDoe,
            StorageLocation = StorageLocationOfJohnDoe,
            ArticleId = 1,
            GlobalTradeItemNumber = "GTIN",
            Name = "Coffee",
            BestBeforeDate = DateTimeProvider.UtcNow,
            Quantity = 10,
            Content = "Capsule",
            ContentType = Core.Persistence.Enums.ContentType.UNKNOWN
        };
        var article2 = new Article
        {
            Household = HouseholdOfJohnDoe,
            StorageLocation = StorageLocationOfJohnDoe,
            ArticleId = 2,
            GlobalTradeItemNumber = "GTIN-2",
            Name = "Coffee Premium",
            BestBeforeDate = DateTimeProvider.UtcNow,
            Quantity = 2,
            Content = "Pack",
            ContentType = Core.Persistence.Enums.ContentType.UNKNOWN
        };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Households.Add(HouseholdOfJohnDoe);
                dbContext.StorageLocations.Add(StorageLocationOfJohnDoe);
                dbContext.Articles.Add(article1);
                dbContext.Articles.Add(article2);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.GetFromJsonAsync<ArticleListResponse>("api/v1/articles", JsonSerializerOptions);

        // Assert
        var articleResponses = response!.Articles;
        if (articleResponses != null)
        {
            articleResponses.Count().ShouldBe(2);
            response.Articles!.First().Id.ShouldBe(article1.ArticleId);
        }
    }

    [Fact]
    public async Task GetArticleByIdAsync_ShouldReturnArticle()
    {
        // Arrange
        var article1 = new Article
        {
            Household = HouseholdOfJohnDoe,
            StorageLocation = StorageLocationOfJohnDoe,
            ArticleId = 1,
            GlobalTradeItemNumber = "GTIN",
            Name = "Coffee",
            BestBeforeDate = DateTimeProvider.UtcNow,
            Quantity = 10,
            Content = "Capsule",
            ContentType = Core.Persistence.Enums.ContentType.UNKNOWN
        };
        var article2 = new Article
        {
            Household = HouseholdOfJohnDoe,
            StorageLocation = StorageLocationOfJohnDoe,
            ArticleId = 2,
            GlobalTradeItemNumber = "GTIN-2",
            Name = "Coffee Premium",
            BestBeforeDate = DateTimeProvider.UtcNow,
            Quantity = 2,
            Content = "Pack",
            ContentType = Core.Persistence.Enums.ContentType.UNKNOWN
        };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Households.Add(HouseholdOfJohnDoe);
                dbContext.StorageLocations.Add(StorageLocationOfJohnDoe);
                dbContext.Articles.Add(article1);
                dbContext.Articles.Add(article2);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.GetFromJsonAsync<ArticleResponse>($"api/v1/articles/{article1.ArticleId}", JsonSerializerOptions);

        // Assert
        response!.BestBeforeDate.ShouldBe(article1.BestBeforeDate);
        response.Content.ShouldBe(article1.Content);
        response.ContentType.ShouldBe(ContentTypeDto.UNKNOWN);
        response.GlobalTradeItemNumber.ShouldBe(article1.GlobalTradeItemNumber);
        response.Id.ShouldBe(article1.ArticleId);
        response.Name.ShouldBe(article1.Name);
        response.Quantity.ShouldBe(article1.Quantity);
        response.StorageLocation.Id.ShouldBe(article1.StorageLocationId);
    }

    [Fact]
    public async Task PostArticleAsync_ShouldReturnArticle()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Households.Add(HouseholdOfJohnDoe);
                dbContext.StorageLocations.Add(StorageLocationOfJohnDoe);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        var expectedArticleRequest = new ArticleRequest
        {
            StorageLocationId = StorageLocationOfJohnDoe.StorageLocationId,
            GlobalTradeItemNumber = "GTIN",
            Name = "Coffee",
            BestBeforeDate = DateTimeProvider.UtcNow,
            Quantity = 10,
            Content = "Capsule",
            ContentType = ContentTypeDto.UNKNOWN
        };

        // Act
        var response = await httpClient.PostAsJsonAsync("api/v1/articles", expectedArticleRequest, JsonSerializerOptions);

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Articles.Count().ShouldBe(1);
            dbContext.Articles.FirstOrDefault()!.HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Articles.FirstOrDefault()!.BestBeforeDate.ShouldBe(expectedArticleRequest.BestBeforeDate);
            dbContext.Articles.FirstOrDefault()!.Content.ShouldBe(expectedArticleRequest.Content);
            dbContext.Articles.FirstOrDefault()!.ContentType.ShouldBe(Core.Persistence.Enums.ContentType.UNKNOWN);
            dbContext.Articles.FirstOrDefault()!.GlobalTradeItemNumber.ShouldBe(expectedArticleRequest.GlobalTradeItemNumber);
            dbContext.Articles.FirstOrDefault()!.ArticleId.ShouldBe(1);
            dbContext.Articles.FirstOrDefault()!.Name.ShouldBe(expectedArticleRequest.Name);
            dbContext.Articles.FirstOrDefault()!.Quantity.ShouldBe(expectedArticleRequest.Quantity);
            dbContext.Articles.FirstOrDefault()!.StorageLocationId.ShouldBe(expectedArticleRequest.StorageLocationId);
        });
    }

    [Fact]
    public async Task PutArticleAsync_ShouldReturnArticle()
    {
        // Arrange
        var article = new Article
        {
            Household = HouseholdOfJohnDoe,
            StorageLocation = StorageLocationOfJohnDoe,
            ArticleId = 1,
            GlobalTradeItemNumber = "GTIN",
            Name = "Coffee",
            BestBeforeDate = DateTimeProvider.UtcNow,
            Quantity = 10,
            Content = "Capsule",
            ContentType = Core.Persistence.Enums.ContentType.UNKNOWN
        };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Households.Add(HouseholdOfJohnDoe);
                dbContext.StorageLocations.Add(StorageLocationOfJohnDoe);
                dbContext.Articles.Add(article);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        var expectedArticleRequest = new ArticleRequest
        {
            StorageLocationId = StorageLocationOfJohnDoe.StorageLocationId,
            GlobalTradeItemNumber = "GTIN-2",
            Name = "Coffee Premium",
            BestBeforeDate = DateTimeProvider.UtcNow,
            Quantity = 2,
            Content = "Pack",
            ContentType = ContentTypeDto.UNKNOWN
        };

        // Act
        var response = await httpClient.PutAsJsonAsync($"api/v1/articles/{article.ArticleId}", expectedArticleRequest);
        await response.Content.ReadAsStringAsync();

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Articles.Count().ShouldBe(1);
            dbContext.Articles.FirstOrDefault()!.HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Articles.FirstOrDefault()!.BestBeforeDate.ShouldBe(expectedArticleRequest.BestBeforeDate);
            dbContext.Articles.FirstOrDefault()!.Content.ShouldBe(expectedArticleRequest.Content);
            dbContext.Articles.FirstOrDefault()!.ContentType.ShouldBe(Core.Persistence.Enums.ContentType.UNKNOWN);
            dbContext.Articles.FirstOrDefault()!.GlobalTradeItemNumber.ShouldBe(expectedArticleRequest.GlobalTradeItemNumber);
            dbContext.Articles.FirstOrDefault()!.ArticleId.ShouldBe(1);
            dbContext.Articles.FirstOrDefault()!.Name.ShouldBe(expectedArticleRequest.Name);
            dbContext.Articles.FirstOrDefault()!.Quantity.ShouldBe(expectedArticleRequest.Quantity);
            dbContext.Articles.FirstOrDefault()!.StorageLocationId.ShouldBe(expectedArticleRequest.StorageLocationId);
        });
    }

    [Fact]
    public async Task DeleteArticleAsync_ShouldWork()
    {
        // Arrange
        var article1 = new Article
        {
            Household = HouseholdOfJohnDoe,
            StorageLocation = StorageLocationOfJohnDoe,
            ArticleId = 1,
            GlobalTradeItemNumber = "GTIN",
            Name = "Coffee",
            BestBeforeDate = DateTimeProvider.UtcNow,
            Quantity = 10,
            Content = "Capsule",
            ContentType = Core.Persistence.Enums.ContentType.UNKNOWN
        };
        var article2 = new Article
        {
            Household = HouseholdOfJohnDoe,
            StorageLocation = StorageLocationOfJohnDoe,
            ArticleId = 2,
            GlobalTradeItemNumber = "GTIN-2",
            Name = "Coffee Premium",
            BestBeforeDate = DateTimeProvider.UtcNow,
            Quantity = 2,
            Content = "Pack",
            ContentType = Core.Persistence.Enums.ContentType.UNKNOWN
        };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Households.Add(HouseholdOfJohnDoe);
                dbContext.StorageLocations.Add(StorageLocationOfJohnDoe);
                dbContext.Articles.Add(article1);
                dbContext.Articles.Add(article2);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.DeleteAsync($"api/v1/articles/{article1.ArticleId}");

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Articles.Count().ShouldBe(1);
            dbContext.Articles.FirstOrDefault()!.HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Articles.FirstOrDefault()!.BestBeforeDate.ShouldBe(article2.BestBeforeDate);
            dbContext.Articles.FirstOrDefault()!.Content.ShouldBe(article2.Content);
            dbContext.Articles.FirstOrDefault()!.ContentType.ShouldBe(Core.Persistence.Enums.ContentType.UNKNOWN);
            dbContext.Articles.FirstOrDefault()!.GlobalTradeItemNumber.ShouldBe(article2.GlobalTradeItemNumber);
            dbContext.Articles.FirstOrDefault()!.ArticleId.ShouldBe(article2.ArticleId);
            dbContext.Articles.FirstOrDefault()!.Name.ShouldBe(article2.Name);
            dbContext.Articles.FirstOrDefault()!.Quantity.ShouldBe(article2.Quantity);
            dbContext.Articles.FirstOrDefault()!.StorageLocationId.ShouldBe(article2.StorageLocationId);
        });
    }
}
