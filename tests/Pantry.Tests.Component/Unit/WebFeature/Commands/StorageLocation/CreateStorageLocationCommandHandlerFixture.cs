using Microsoft.Extensions.Logging;
using Pantry.Core.Persistence;
using Pantry.Features.WebFeature.Commands;

namespace Pantry.Tests.Component.Unit.WebFeature.Commands;

[Trait("Category", "Unit")]
public class CreateStorageLocationCommandHandlerFixture : BaseFixture
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
        });
        var commandHandler = new CreateStorageLocationCommandHandler(
            Substitute.For<ILogger<CreateStorageLocationCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoeWithHousehold);

        // Act
        var act = await commandHandler.ExecuteAsync(new CreateStorageLocationCommand("UnitTest", "FooBar Description"));

        // Assert
        act.StorageLocationId.ShouldBe(1);
        act.Name.ShouldBe("UnitTest");
        act.Description.ShouldBe("FooBar Description");
        act.HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
    }
}
