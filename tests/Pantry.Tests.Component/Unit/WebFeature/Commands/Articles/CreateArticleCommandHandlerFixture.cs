using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pantry.Common.Time;
using Pantry.Core.Persistence;
using Pantry.Features.WebFeature.Commands;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;

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
        act.StorageLocationId.Should().Be(StorageLocationOfJohnDoe.StorageLocationId);
        act.HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
        act.ArticleId.Should().Be(1);
        act.GlobalTradeItemNumber.Should().Be("GTIN");
        act.Name.Should().Be("Coffee");
        act.BestBeforeDate.Should().Be(DateTimeProvider.UtcNow);
        act.Quantity.Should().Be(10);
        act.Content.Should().Be("Capsule");
        act.ContentType.Should().Be(Core.Persistence.Enums.ContentType.UNKNOWN);
    }
}
