using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Pantry.Common.Time;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Enums;
using Pantry.Features.WebFeature.V1.Controllers.Requests;
using Pantry.Features.WebFeature.V1.Controllers.Responses;
using Pantry.Tests.Component.Integration.Environment;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Xunit;
using Xunit.Abstractions;

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
        testApplication.SetupDatabase<AppDbContext>(
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
        response!.Articles.Should().HaveCount(2);
        response!.Articles!.First().Id.Should().Be(article1.ArticleId);
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
        testApplication.SetupDatabase<AppDbContext>(
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
        response!.BestBeforeDate.Should().Be(article1.BestBeforeDate);
        response!.Content.Should().Be(article1.Content);
        response!.ContentType.Should().Be(ContentType.UNKNOWN);
        response!.GlobalTradeItemNumber.Should().Be(article1.GlobalTradeItemNumber);
        response!.Id.Should().Be(article1.ArticleId);
        response!.Name.Should().Be(article1.Name);
        response!.Quantity.Should().Be(article1.Quantity);
        response!.StorageLocation.Id.Should().Be(article1.StorageLocationId);
    }

    [Fact]
    public async Task PostArticleAsync_ShouldReturnArticle()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
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
            ContentType = ContentType.UNKNOWN
        };

        // Act
        var response = await httpClient.PostAsJsonAsync<ArticleRequest>("api/v1/articles", expectedArticleRequest, JsonSerializerOptions);

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Articles.Count().Should().Be(1);
            dbContext.Articles.FirstOrDefault()!.HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Articles.FirstOrDefault()!.BestBeforeDate.Should().Be(expectedArticleRequest.BestBeforeDate);
            dbContext.Articles.FirstOrDefault()!.Content.Should().Be(expectedArticleRequest.Content);
            dbContext.Articles.FirstOrDefault()!.ContentType.Should().Be(Core.Persistence.Enums.ContentType.UNKNOWN);
            dbContext.Articles.FirstOrDefault()!.GlobalTradeItemNumber.Should().Be(expectedArticleRequest.GlobalTradeItemNumber);
            dbContext.Articles.FirstOrDefault()!.ArticleId.Should().Be(1);
            dbContext.Articles.FirstOrDefault()!.Name.Should().Be(expectedArticleRequest.Name);
            dbContext.Articles.FirstOrDefault()!.Quantity.Should().Be(expectedArticleRequest.Quantity);
            dbContext.Articles.FirstOrDefault()!.StorageLocationId.Should().Be(expectedArticleRequest.StorageLocationId);
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
        testApplication.SetupDatabase<AppDbContext>(
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
            ContentType = ContentType.UNKNOWN
        };

        // Act
        var response = await httpClient.PutAsJsonAsync<ArticleRequest>($"api/v1/articles/{article.ArticleId}", expectedArticleRequest);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Articles.Count().Should().Be(1);
            dbContext.Articles.FirstOrDefault()!.HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Articles.FirstOrDefault()!.BestBeforeDate.Should().Be(expectedArticleRequest.BestBeforeDate);
            dbContext.Articles.FirstOrDefault()!.Content.Should().Be(expectedArticleRequest.Content);
            dbContext.Articles.FirstOrDefault()!.ContentType.Should().Be(Core.Persistence.Enums.ContentType.UNKNOWN);
            dbContext.Articles.FirstOrDefault()!.GlobalTradeItemNumber.Should().Be(expectedArticleRequest.GlobalTradeItemNumber);
            dbContext.Articles.FirstOrDefault()!.ArticleId.Should().Be(1);
            dbContext.Articles.FirstOrDefault()!.Name.Should().Be(expectedArticleRequest.Name);
            dbContext.Articles.FirstOrDefault()!.Quantity.Should().Be(expectedArticleRequest.Quantity);
            dbContext.Articles.FirstOrDefault()!.StorageLocationId.Should().Be(expectedArticleRequest.StorageLocationId);
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
        testApplication.SetupDatabase<AppDbContext>(
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
            dbContext.Articles.Count().Should().Be(1);
            dbContext.Articles.FirstOrDefault()!.HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
            dbContext.Articles.FirstOrDefault()!.BestBeforeDate.Should().Be(article2.BestBeforeDate);
            dbContext.Articles.FirstOrDefault()!.Content.Should().Be(article2.Content);
            dbContext.Articles.FirstOrDefault()!.ContentType.Should().Be(Core.Persistence.Enums.ContentType.UNKNOWN);
            dbContext.Articles.FirstOrDefault()!.GlobalTradeItemNumber.Should().Be(article2.GlobalTradeItemNumber);
            dbContext.Articles.FirstOrDefault()!.ArticleId.Should().Be(article2.ArticleId);
            dbContext.Articles.FirstOrDefault()!.Name.Should().Be(article2.Name);
            dbContext.Articles.FirstOrDefault()!.Quantity.Should().Be(article2.Quantity);
            dbContext.Articles.FirstOrDefault()!.StorageLocationId.Should().Be(article2.StorageLocationId);
        });
    }
}
