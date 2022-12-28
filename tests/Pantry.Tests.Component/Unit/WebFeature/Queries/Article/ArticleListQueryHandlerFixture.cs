using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pantry.Common.Time;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Queries;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;

namespace Pantry.Tests.Component.Unit.WebFeature.Queries;

[Trait("Category", "Unit")]
public class ArticleListQueryHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturn()
    {
        // Arrange
        var article1 = new Article
        {
            Household = HouseholdOfJohnDoe,
            StorageLocation = StorageLocationOfJohnDoe,
            GlobalTradeItemNumber = "GTIN-Coffee",
            Name = "Coffee",
            BestBeforeDate = DateTimeProvider.UtcNow,
            Quantity = 3,
            Content = "Capsule",
            ContentType = Core.Persistence.Enums.ContentType.UNKNOWN
        };
        var article2 = new Article
        {
            Household = HouseholdOfJohnDoe,
            StorageLocation = StorageLocationOfJohnDoe,
            GlobalTradeItemNumber = "GTIN-Chocolate",
            Name = "Chocolate",
            BestBeforeDate = DateTimeProvider.UtcNow,
            Quantity = 10,
            Content = "Bars",
            ContentType = Core.Persistence.Enums.ContentType.UNKNOWN
        };
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.StorageLocations.Add(StorageLocationOfJohnDoe);
            dbContext.Articles.Add(article1);
            dbContext.Articles.Add(article2);
        });

        var queryHandler = new ArticleListQueryHandler(
            Substitute.For<ILogger<ArticleListQueryHandler>>(),
            testDatabase,
            PrincipalOfJohnDoeWithHousehold);

        // Act
        IReadOnlyCollection<Article> articles = await queryHandler.ExecuteAsync(new ArticleListQuery());

        // Assert
        articles.Should().HaveCount(2);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnEmpty()
    {
        // Arrange
        var article1 = new Article
        {
            Household = HouseholdOfJohnDoe,
            StorageLocation = StorageLocationOfJohnDoe,
            GlobalTradeItemNumber = "GTIN-Coffee",
            Name = "Coffee",
            BestBeforeDate = DateTimeProvider.UtcNow,
            Quantity = 3,
            Content = "Capsule",
            ContentType = Core.Persistence.Enums.ContentType.UNKNOWN
        };
        var article2 = new Article
        {
            Household = HouseholdOfJohnDoe,
            StorageLocation = StorageLocationOfJohnDoe,
            GlobalTradeItemNumber = "GTIN-Chocolate",
            Name = "Chocolate",
            BestBeforeDate = DateTimeProvider.UtcNow,
            Quantity = 10,
            Content = "Bars",
            ContentType = Core.Persistence.Enums.ContentType.UNKNOWN
        };
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(AccountJohnDoe);
            dbContext.Households.Add(HouseholdOfJohnDoe);
            dbContext.StorageLocations.Add(StorageLocationOfJohnDoe);
            dbContext.Articles.Add(article1);
            dbContext.Articles.Add(article2);
        });

        var queryHandler = new ArticleListQueryHandler(
            Substitute.For<ILogger<ArticleListQueryHandler>>(),
            testDatabase,
            PrincipalOfJohnDoe);

        // Act
        IReadOnlyCollection<Article> articles = await queryHandler.ExecuteAsync(new ArticleListQuery());

        // Assert
        articles.Should().HaveCount(0);
    }
}
