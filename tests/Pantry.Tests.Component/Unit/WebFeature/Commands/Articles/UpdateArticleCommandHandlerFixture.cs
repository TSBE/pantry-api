using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pantry.Common.EntityFrameworkCore.Exceptions;
using Pantry.Common.Time;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Commands;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;

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
        act.StorageLocationId.Should().Be(StorageLocationOfJohnDoe.StorageLocationId);
        act.HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
        act.ArticleId.Should().Be(1);
        act.GlobalTradeItemNumber.Should().Be("GTIN-Updated");
        act.Name.Should().Be("Coffee Updated");
        act.BestBeforeDate.Should().Be(article.BestBeforeDate);
        act.Quantity.Should().Be(42);
        act.Content.Should().Be("Capsule");
        act.ContentType.Should().Be(Core.Persistence.Enums.ContentType.UNKNOWN);
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
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }
}
