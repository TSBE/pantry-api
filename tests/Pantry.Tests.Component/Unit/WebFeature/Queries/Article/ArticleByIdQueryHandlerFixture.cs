using Microsoft.Extensions.Logging;
using Pantry.Common.EntityFrameworkCore.Exceptions;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Queries;

namespace Pantry.Tests.Component.Unit.WebFeature.Queries;

[Trait("Category", "Unit")]
public class ArticleByIdQueryHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturn()
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
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.StorageLocations.Add(StorageLocationOfJohnDoe);
            dbContext.Articles.Add(article);
        });

        var queryHandler = new ArticleByIdQueryHandler(Substitute.For<ILogger<ArticleByIdQueryHandler>>(), testDatabase, PrincipalOfJohnDoeWithHousehold);

        // Act
        Article actual = await queryHandler.ExecuteAsync(new ArticleByIdQuery(article.ArticleId));

        // Assert
        actual.StorageLocationId.ShouldBe(article.StorageLocationId);
        actual.HouseholdId.ShouldBe(article.HouseholdId);
        actual.ArticleId.ShouldBe(article.ArticleId);
        actual.GlobalTradeItemNumber.ShouldBe(article.GlobalTradeItemNumber);
        actual.Name.ShouldBe(article.Name);
        actual.BestBeforeDate.ShouldBe(article.BestBeforeDate);
        actual.Quantity.ShouldBe(article.Quantity);
        actual.Content.ShouldBe(article.Content);
        actual.ContentType.ShouldBe(article.ContentType);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnMetadata()
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
        var metadata = new Metadata { GlobalTradeItemNumber = article.GlobalTradeItemNumber, FoodFacts = new Core.Models.OpenFoodFacts.Product { Brands = "Unittest" } };
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.StorageLocations.Add(StorageLocationOfJohnDoe);
            dbContext.Articles.Add(article);
            dbContext.Metadatas.Add(metadata);
        });

        var queryHandler = new ArticleByIdQueryHandler(Substitute.For<ILogger<ArticleByIdQueryHandler>>(), testDatabase, PrincipalOfJohnDoeWithHousehold);

        // Act
        Article actual = await queryHandler.ExecuteAsync(new ArticleByIdQuery(article.ArticleId));

        // Assert
        actual.StorageLocationId.ShouldBe(article.StorageLocationId);
        actual.HouseholdId.ShouldBe(article.HouseholdId);
        actual.ArticleId.ShouldBe(article.ArticleId);
        actual.GlobalTradeItemNumber.ShouldBe(article.GlobalTradeItemNumber);
        actual.Name.ShouldBe(article.Name);
        actual.BestBeforeDate.ShouldBe(article.BestBeforeDate);
        actual.Quantity.ShouldBe(article.Quantity);
        actual.Content.ShouldBe(article.Content);
        actual.ContentType.ShouldBe(article.ContentType);
        actual.Metadata?.FoodFacts?.Brands.ShouldBe(metadata.FoodFacts.Brands);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnWithoutMetadata()
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
        var metadata = new Metadata { GlobalTradeItemNumber = "GTIN2", FoodFacts = new Core.Models.OpenFoodFacts.Product { Brands = "Unittest" } };
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.StorageLocations.Add(StorageLocationOfJohnDoe);
            dbContext.Articles.Add(article);
            dbContext.Metadatas.Add(metadata);
        });

        var queryHandler = new ArticleByIdQueryHandler(Substitute.For<ILogger<ArticleByIdQueryHandler>>(), testDatabase, PrincipalOfJohnDoeWithHousehold);

        // Act
        Article actual = await queryHandler.ExecuteAsync(new ArticleByIdQuery(article.ArticleId));

        // Assert
        actual.StorageLocationId.ShouldBe(article.StorageLocationId);
        actual.HouseholdId.ShouldBe(article.HouseholdId);
        actual.ArticleId.ShouldBe(article.ArticleId);
        actual.GlobalTradeItemNumber.ShouldBe(article.GlobalTradeItemNumber);
        actual.Name.ShouldBe(article.Name);
        actual.BestBeforeDate.ShouldBe(article.BestBeforeDate);
        actual.Quantity.ShouldBe(article.Quantity);
        actual.Content.ShouldBe(article.Content);
        actual.ContentType.ShouldBe(article.ContentType);
        actual.Metadata.ShouldBeNull();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow()
    {
        // Arrange
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.StorageLocations.Add(StorageLocationOfJohnDoe);
        });

        var queryHandler = new ArticleByIdQueryHandler(Substitute.For<ILogger<ArticleByIdQueryHandler>>(), testDatabase, PrincipalOfJohnDoeWithHousehold);

        // Act
        Func<Task> act = async () => await queryHandler.ExecuteAsync(new ArticleByIdQuery(1));

        // Assert
        await act.ShouldThrowAsync<EntityNotFoundException>();
    }
}
