using Microsoft.Extensions.Logging;
using Pantry.Core.Persistence;
using Pantry.Features.WebFeature.Commands;

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
        act.InstallationId.ShouldBe(installationId);
        act.Model.ShouldBeEquivalentTo("iPhone 14");
        act.Name.ShouldBeEquivalentTo("Foo`s iPhone");
        act.Platform.ShouldBe(Core.Persistence.Enums.DevicePlatformType.IOS);
        act.DeviceToken.ShouldBeNull();
    }
}
