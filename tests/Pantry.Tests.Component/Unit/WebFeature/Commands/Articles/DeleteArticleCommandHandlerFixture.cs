using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
public class DeleteArticleCommandHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldDeleteArticle()
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
        var commandHandler = new DeleteArticleCommandHandler(
            Substitute.For<ILogger<DeleteArticleCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoeWithHousehold);

        // Act
        await commandHandler.ExecuteAsync(new DeleteArticleCommand(article1.ArticleId));

        // Assert
        testDatabase.AssertDatabaseContent(
            dbContext =>
            {
                dbContext.Accounts.Should().HaveCount(1);
                dbContext.Households.Should().HaveCount(1);
                dbContext.StorageLocations.Should().HaveCount(1);
                dbContext.Articles.Should().HaveCount(1);
                dbContext.Articles.FirstOrDefault()!.ArticleId.Should().Be(article2.ArticleId);
            });
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
        var commandHandler = new DeleteArticleCommandHandler(
            Substitute.For<ILogger<DeleteArticleCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoeWithHousehold);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new DeleteArticleCommand(1));

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }
}
