using Microsoft.Extensions.Logging;
using Pantry.Core.Persistence;
using Pantry.Features.WebFeature.Commands;

namespace Pantry.Tests.Component.Unit.WebFeature.Commands;

[Trait("Category", "Unit")]
public class CreateArticleCommandHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldCreateStorageLocation()
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
        var commandHandler = new CreateArticleCommandHandler(
            Substitute.For<ILogger<CreateArticleCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoeWithHousehold);

        using var context = new DateTimeContext(DateTimeProvider.UtcNow);

        // Act
        var act = await commandHandler.ExecuteAsync(new CreateArticleCommand(
            StorageLocationId: StorageLocationOfJohnDoe.StorageLocationId,
            GlobalTradeItemNumber: "GTIN",
            Name: "Coffee",
            BestBeforeDate: DateTimeProvider.UtcNow,
            Quantity: 10,
            Content: "Capsule",
            ContentType: Core.Persistence.Enums.ContentType.UNKNOWN));

        // Assert
        act.StorageLocationId.ShouldBe(StorageLocationOfJohnDoe.StorageLocationId);
        act.HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
        act.ArticleId.ShouldBe(1);
        act.GlobalTradeItemNumber.ShouldBe("GTIN");
        act.Name.ShouldBe("Coffee");
        act.BestBeforeDate.ShouldBe(DateTimeProvider.UtcNow);
        act.Quantity.ShouldBe(10);
        act.Content.ShouldBe("Capsule");
        act.ContentType.ShouldBe(Core.Persistence.Enums.ContentType.UNKNOWN);
    }
}
