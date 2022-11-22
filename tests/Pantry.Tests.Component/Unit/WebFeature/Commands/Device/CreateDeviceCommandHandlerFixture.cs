using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Commands;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;

namespace Pantry.Tests.Component.Unit.WebFeature.Commands;

[Trait("Category", "Unit")]
public class CreateDeviceCommandHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldCreateDevice()
    {
        // Arrange
        var account = new Account { AccountId = 1, FirstName = "Jane", LastName = "Doe", FriendsCode = Guid.NewGuid(), OAuhtId = "dummy" };
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
            dbContext =>
            {
                dbContext.Accounts.Add(account);
            });
        var commandHandler = new CreateDeviceCommandHandler(
            Substitute.For<ILogger<CreateDeviceCommandHandler>>(),
            testDatabase);

        // Act
        // var act = await commandHandler.ExecuteAsync(new CreateDeviceCommand(null, Guid.NewGuid(), "iPhone 14", "Foo`s iPhone", Core.Persistence.Enums.DevicePlatformType.IOS));

        // Assert
        // act.Name.Should().BeEquivalentTo("Foo`s iPhone");
    }
}
