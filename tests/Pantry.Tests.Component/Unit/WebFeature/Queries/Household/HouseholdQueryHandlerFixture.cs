using Microsoft.Extensions.Logging;
using Pantry.Common.EntityFrameworkCore.Exceptions;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Core.Persistence.Enums;
using Pantry.Features.WebFeature.Queries;

namespace Pantry.Tests.Component.Unit.WebFeature.Queries;

[Trait("Category", "Unit")]
public class HouseholdQueryHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturn()
    {
        // Arrange
        var household = new Household { Name = "Test", SubscriptionType = SubscriptionType.FREE };
        AccountJohnDoe.Household = household;
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Households.Add(household);
        });

        var queryHandler = new HouseholdQueryHandler(Substitute.For<ILogger<HouseholdQueryHandler>>(), testDatabase, PrincipalOfJohnDoeWithHousehold);

        // Act
        Household actual = await queryHandler.ExecuteAsync(new HouseholdQuery());

        // Assert
        actual.Name.ShouldBe(household.Name);
        actual.SubscriptionType.ShouldBe(household.SubscriptionType);
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
        });

        var queryHandler = new HouseholdQueryHandler(Substitute.For<ILogger<HouseholdQueryHandler>>(), testDatabase, PrincipalOfJohnDoe);

        // Act
        Func<Task> act = async () => await queryHandler.ExecuteAsync(new HouseholdQuery());

        // Assert
        await act.ShouldThrowAsync<EntityNotFoundException>();
    }
}
