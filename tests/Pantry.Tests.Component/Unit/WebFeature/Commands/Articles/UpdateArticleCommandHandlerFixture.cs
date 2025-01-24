using Microsoft.Extensions.Logging;
using Pantry.Common.EntityFrameworkCore.Exceptions;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Commands;

namespace Pantry.Tests.Component.Unit.WebFeature.Commands;

[Trait("Category", "Unit")]
public class UpdateArticleCommandHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldUpdateArticle()
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
        var commandHandler = new UpdateArticleCommandHandler(
            Substitute.For<ILogger<UpdateArticleCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoeWithHousehold);

        // Act
        var act = await commandHandler.ExecuteAsync(new UpdateArticleCommand(
            ArticleId: article.ArticleId,
            StorageLocationId: article.StorageLocationId,
            GlobalTradeItemNumber: "GTIN-Updated",
            Name: "Coffee Updated",
            BestBeforeDate: article.BestBeforeDate,
            Quantity: 42,
            Content: "Capsule",
            ContentType: Core.Persistence.Enums.ContentType.UNKNOWN));

        // Assert
        act.StorageLocationId.ShouldBe(StorageLocationOfJohnDoe.StorageLocationId);
        act.HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
        act.ArticleId.ShouldBe(1);
        act.GlobalTradeItemNumber.ShouldBe("GTIN-Updated");
        act.Name.ShouldBe("Coffee Updated");
        act.BestBeforeDate.ShouldBe(article.BestBeforeDate);
        act.Quantity.ShouldBe(42);
        act.Content.ShouldBe("Capsule");
        act.ContentType.ShouldBe(Core.Persistence.Enums.ContentType.UNKNOWN);
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
        var commandHandler = new UpdateArticleCommandHandler(
            Substitute.For<ILogger<UpdateArticleCommandHandler>>(),
            testDatabase,
            PrincipalOfFooBarWithHousehold);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new UpdateArticleCommand(
            ArticleId: 1,
            StorageLocationId: StorageLocationOfJohnDoe.StorageLocationId,
            GlobalTradeItemNumber: "GTIN-Updated",
            Name: "Coffee Updated",
            BestBeforeDate: DateTimeProvider.UtcNow,
            Quantity: 42,
            Content: "Capsule",
            ContentType: Core.Persistence.Enums.ContentType.UNKNOWN));

        // Assert
        await act.ShouldThrowAsync<EntityNotFoundException>();
    }
}
