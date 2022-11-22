using System.Collections.Generic;
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
public class DeviceListQueryHandlerFixture : BaseFixture
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturn()
    {
        //using SqliteInMemoryDbContextFactory<AppDbContext> testDatabase = new();
        //testDatabase.SetupDatabase(
        //    dbContext =>
        //    {
        //        dbContext.Devices.Add(new Device { Name = "NotNullName", Model = "NotNullModel" });
        //    });

        //var queryHandler = new DeviceListQueryHandler(
        //    Substitute.For<ILogger<DeviceListQueryHandler>>(),
        //    testDatabase);

        //// Act
        //IReadOnlyCollection<Device> reports = await queryHandler.ExecuteAsync(new DeviceListQuery());

        //// Assert
        //reports.Should().HaveCount(1);
    }
}
