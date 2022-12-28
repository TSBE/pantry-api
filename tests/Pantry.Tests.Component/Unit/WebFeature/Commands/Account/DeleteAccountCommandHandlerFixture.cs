using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pantry.Common.EntityFrameworkCore.Exceptions;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Commands;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;

namespace Pantry.Tests.Component.Unit.WebFeature.Commands;

[Trait("Category", "Unit")]
public class DeleteAccountCommandHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldDeleteDevice()
    {
        // Arrange
        var account = new Account { AccountId = 1, FirstName = "Jane", LastName = "Doe", FriendsCode = Guid.NewGuid(), OAuhtId = PrincipalJohnDoeId };
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
        dbContext =>
        {
            dbContext.Accounts.Add(account);
        });
        var commandHandler = new DeleteAccountCommandHandler(
            Substitute.For<ILogger<DeleteAccountCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoe);

        // Act
        await commandHandler.ExecuteAsync(new DeleteAccountCommand());

        // Assert
        testDatabase.AssertDatabaseContent(
            dbContext =>
            {
                dbContext.Accounts.Should().HaveCount(0);
            });
    }

    [Fact]
    public async Task ExecuteAsync_ShouldThrow()
    {
        // Arrange
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase();
        var commandHandler = new DeleteAccountCommandHandler(
            Substitute.For<ILogger<DeleteAccountCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoe);

        // Act
        Func<Task> act = async () => await commandHandler.ExecuteAsync(new DeleteAccountCommand());

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }
}
