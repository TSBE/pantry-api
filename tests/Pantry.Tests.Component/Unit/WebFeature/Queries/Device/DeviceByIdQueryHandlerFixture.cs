using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.Queries;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Pantry.Tests.EntityFrameworkCore.Persistence;
using Xunit;

namespace Pantry.Tests.Component.Unit.WebFeature.Queries;

[Trait("Category", "Unit")]
public class DeviceByIdQueryHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturn()
    {
        using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        testDatabase.SetupDatabase(
            dbContext =>
            {
                dbContext.Devices.Add(new Device { DeviceId = 123, Name = "NotNullName", Model = "NotNullModel" });
            });

        var queryHandler = new DeviceByIdQueryHandler(Substitute.For<ILogger<DeviceByIdQueryHandler>>(), testDatabase);

        // Act
        Device actualReport = await queryHandler.ExecuteAsync(new DeviceByIdQuery(123));

        // Assert
        actualReport.Should().NotBeNull();
    }
}
