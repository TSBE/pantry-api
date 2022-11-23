using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pantry.Common.EntityFrameworkCore.Exceptions;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Queries;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;

namespace Pantry.Tests.Component.Unit.WebFeature.Queries;

[Trait("Category", "Unit")]
public class AccountQueryHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturn()
    {
        // Arrange
        var account = new Account { AccountId = 1, FirstName = "Jane", LastName = "Doe", FriendsCode = Guid.NewGuid(), OAuhtId = PrincipalJohnDoeId };
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(account);
        });

        var queryHandler = new AccountQueryHandler(Substitute.For<ILogger<AccountQueryHandler>>(), testDatabase, PrincipalOfJohnDoe);

        // Act
        Account actual = await queryHandler.ExecuteAsync(new AccountQuery());

        // Assert
        actual.Should().BeEquivalentTo(account);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow()
    {
        // Arrange
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase();

        var queryHandler = new AccountQueryHandler(Substitute.For<ILogger<AccountQueryHandler>>(), testDatabase, PrincipalOfJohnDoe);

        // Act
        Func<Task> act = async () => await queryHandler.ExecuteAsync(new AccountQuery());

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }
}
