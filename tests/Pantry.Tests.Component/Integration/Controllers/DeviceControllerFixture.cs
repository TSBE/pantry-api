using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Enums;
using Pantry.Features.WebFeature.V1.Controllers.Requests;
using Pantry.Features.WebFeature.V1.Controllers.Responses;
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
    public async Task GetDeviceListAsync_ShouldReturnDevice()
    {
        // Arrange
        var device1 = new Device { Account = AccountJohnDoe, InstallationId = Guid.NewGuid(), Name = "iPhone 14", Model = "Foo`s iPhone", Platform = Core.Persistence.Enums.DevicePlatformType.IOS };
        var device2 = new Device { Account = AccountJohnDoe, InstallationId = Guid.NewGuid(), Name = "Samsung Galaxy S22", Model = "Foo`s Galaxy", Platform = Core.Persistence.Enums.DevicePlatformType.ANDROID };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Devices.Add(device1);
                dbContext.Devices.Add(device2);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.GetFromJsonAsync<DeviceListResponse>("api/v1/devices", JsonSerializerOptions);

        // Assert
        response!.Devices.Should().HaveCount(2);
        response!.Devices!.First().InstallationId.Should().Be(device1.InstallationId);
    }

    [Fact]
    public async Task GetDeviceByIdAsync_ShouldReturnDevice()
    {
        // Arrange
        var device1 = new Device { Account = AccountJohnDoe, InstallationId = Guid.NewGuid(), Name = "iPhone 14", Model = "Foo`s iPhone", Platform = Core.Persistence.Enums.DevicePlatformType.IOS };
        var device2 = new Device { Account = AccountJohnDoe, InstallationId = Guid.NewGuid(), Name = "Samsung Galaxy S22", Model = "Foo`s Galaxy", Platform = Core.Persistence.Enums.DevicePlatformType.ANDROID };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Devices.Add(device1);
                dbContext.Devices.Add(device2);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.GetFromJsonAsync<DeviceResponse>($"api/v1/devices/{device1.InstallationId}", JsonSerializerOptions);

        // Assert
        response!.InstallationId.Should().Be(device1.InstallationId);
    }

    [Fact]
    public async Task PostDeviceAsync_ShouldReturnDevice()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
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

    [Fact]
    public async Task PutDeviceAsync_ShouldReturnDevice()
    {
        // Arrange
        var device = new Device { Account = AccountJohnDoe, InstallationId = Guid.NewGuid(), Name = "iPhone 14", Model = "Foo`s iPhone", Platform = Core.Persistence.Enums.DevicePlatformType.IOS };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Devices.Add(device);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        var expectedDeviceeRequest = new DeviceUpdateRequest
        {
            Name = "Integration Phone",
            DeviceToken = "Integration Token"
        };

        // Act
        var response = await httpClient.PutAsJsonAsync($"api/v1/devices/{device.InstallationId}", expectedDeviceeRequest);

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Devices.Count().Should().Be(1);
            dbContext.Devices.FirstOrDefault()!.Name.Should().Be(expectedDeviceeRequest.Name);
            dbContext.Devices.FirstOrDefault()!.DeviceToken.Should().Be(expectedDeviceeRequest.DeviceToken);
            dbContext.Devices.FirstOrDefault()!.InstallationId.Should().Be(device.InstallationId);
            dbContext.Devices.FirstOrDefault()!.Model.Should().Be(device.Model);
            dbContext.Devices.FirstOrDefault()!.Platform.Should().Be(device.Platform);
            dbContext.Devices.FirstOrDefault()!.DeviceId.Should().Be(1);
        });
    }

    [Fact]
    public async Task DeleteDeviceAsync_ShouldWork()
    {
        // Arrange
        var device1 = new Device { Account = AccountJohnDoe, InstallationId = Guid.NewGuid(), Name = "iPhone 14", Model = "Foo`s iPhone", Platform = Core.Persistence.Enums.DevicePlatformType.IOS };
        var device2 = new Device { Account = AccountJohnDoe, InstallationId = Guid.NewGuid(), Name = "Samsung Galaxy S22", Model = "Foo`s Galaxy", Platform = Core.Persistence.Enums.DevicePlatformType.ANDROID };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Devices.Add(device1);
                dbContext.Devices.Add(device2);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.DeleteAsync($"api/v1/devices/{device1.InstallationId}");

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.Devices.Count().Should().Be(1);
            dbContext.Devices.FirstOrDefault()!.Name.Should().Be(device2.Name);
            dbContext.Devices.FirstOrDefault()!.DeviceToken.Should().Be(device2.DeviceToken);
            dbContext.Devices.FirstOrDefault()!.InstallationId.Should().Be(device2.InstallationId);
            dbContext.Devices.FirstOrDefault()!.Model.Should().Be(device2.Model);
            dbContext.Devices.FirstOrDefault()!.Platform.Should().Be(device2.Platform);
            dbContext.Devices.FirstOrDefault()!.DeviceId.Should().Be(2);
        });
    }
}
