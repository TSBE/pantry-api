using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Pantry.Core.Persistence;
using Pantry.Features.WebFeature.V1.Controllers.Enums;
using Pantry.Features.WebFeature.V1.Controllers.Requests;
using Pantry.Tests.Component.Integration.Environment;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Pantry.Tests.Component.Integration.Controllers;

[Trait("Category", "Integration")]
public class DeviceControllerFixture : BaseControllerFixture
{
    public DeviceControllerFixture(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task PostDeviceAsync_ShouldReturnDevice()
    {
        // Arrange

        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper, services =>
        {
        });

        using HttpClient httpClient = testApplication.CreateClient();

        var expectedDeviceeRequest = new DeviceRequest
        {
            InstallationId = Guid.NewGuid(),
            Model = "Integration Test",
            Name = "Integration Phone",
            Platform = DevicePlatformType.ANDROID
        };

        // Act
        var response = await httpClient.PostAsJsonAsync<DeviceRequest>("api/v1/devices", expectedDeviceeRequest);

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Devices.Count().Should().Be(1);
            dbContext.Devices.FirstOrDefault()!.InstallationId.Should().Be(expectedDeviceeRequest.InstallationId);
            dbContext.Devices.FirstOrDefault()!.Model.Should().Be(expectedDeviceeRequest.Model);
            dbContext.Devices.FirstOrDefault()!.Name.Should().Be(expectedDeviceeRequest.Name);
            dbContext.Devices.FirstOrDefault()!.Platform.Should().Be(Core.Persistence.Enums.DevicePlatformType.ANDROID);
            dbContext.Devices.FirstOrDefault()!.DeviceId.Should().Be(1);
        });
    }
}
