using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pantry.Core.Persistence;
using Pantry.Features.WebFeature.Commands;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;

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
        act.StorageLocationId.Should().Be(1);
        act.Name.Should().Be("UnitTest");
        act.Description.Should().Be("FooBar Description");
        act.HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
    }
}
