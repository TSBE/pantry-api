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
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
            });
        var commandHandler = new CreateDeviceCommandHandler(
            Substitute.For<ILogger<CreateDeviceCommandHandler>>(),
            testDatabase,
            PrincipalOfJohnDoe);

        var installationId = Guid.NewGuid();

        // Act
        var act = await commandHandler.ExecuteAsync(new CreateDeviceCommand(installationId, "iPhone 14", "Foo`s iPhone", Core.Persistence.Enums.DevicePlatformType.IOS, null));

        // Assert
        act.InstallationId.Should().Be(installationId);
        act.Model.Should().BeEquivalentTo("iPhone 14");
        act.Name.Should().BeEquivalentTo("Foo`s iPhone");
        act.Platform.Should().Be(Core.Persistence.Enums.DevicePlatformType.IOS);
        act.DeviceToken.Should().BeNull();
    }
}
