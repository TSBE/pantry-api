using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Requests;
using Pantry.Features.WebFeature.V1.Controllers.Responses;
using Pantry.Tests.Component.Integration.Environment;
using Pantry.Tests.EntityFrameworkCore.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Pantry.Tests.Component.Integration.Controllers;

[Trait("Category", "Integration")]
public class StorageLocationControllerFixture : BaseControllerFixture
{
    public StorageLocationControllerFixture(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task GetStorageLocationListAsync_ShouldReturnStorageLocations()
    {
        // Arrange
        var storageLocation1 = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 1, Name = "Test Location", Description = "Bar Description" };
        var storageLocation2 = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 2, Name = "Unit Location", Description = "Foo Description" };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Households.Add(HouseholdOfJohnDoe);
                dbContext.StorageLocations.Add(storageLocation1);
                dbContext.StorageLocations.Add(storageLocation2);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.GetFromJsonAsync<StorageLocationListResponse>("api/v1/storage-locations", JsonSerializerOptions);

        // Assert
        response!.StorageLocations.Should().HaveCount(2);
        response!.StorageLocations!.First().Id.Should().Be(storageLocation1.StorageLocationId);
    }

    [Fact]
    public async Task GetStorageLocationByIdAsync_ShouldReturnStorageLocation()
    {
        // Arrange
        var storageLocation1 = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 1, Name = "Test Location", Description = "Bar Description" };
        var storageLocation2 = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 2, Name = "Unit Location", Description = "Foo Description" };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Households.Add(HouseholdOfJohnDoe);
                dbContext.StorageLocations.Add(storageLocation1);
                dbContext.StorageLocations.Add(storageLocation2);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.GetFromJsonAsync<StorageLocationResponse>($"api/v1/storage-locations/{storageLocation1.StorageLocationId}", JsonSerializerOptions);

        // Assert
        response!.Id.Should().Be(storageLocation1.StorageLocationId);
    }

    [Fact]
    public async Task PostStorageLocationAsync_ShouldReturnStorageLocation()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Households.Add(HouseholdOfJohnDoe);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        var expectedStorageLocationRequest = new StorageLocationRequest
        {
            Name = "Test Location",
            Description = "Bar Description"
        };

        // Act
        var response = await httpClient.PostAsJsonAsync<StorageLocationRequest>("api/v1/storage-locations", expectedStorageLocationRequest);

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.StorageLocations.Count().Should().Be(1);
            dbContext.StorageLocations.FirstOrDefault()!.StorageLocationId.Should().Be(1);
            dbContext.StorageLocations.FirstOrDefault()!.Name.Should().Be(expectedStorageLocationRequest.Name);
            dbContext.StorageLocations.FirstOrDefault()!.Description.Should().Be(expectedStorageLocationRequest.Description);
            dbContext.StorageLocations.FirstOrDefault()!.HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
        });
    }

    [Fact]
    public async Task PutStorageLocationAsync_ShouldReturnStorageLocation()
    {
        // Arrange
        var storageLocation = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 1, Name = "Test Location", Description = "Bar Description" };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Households.Add(HouseholdOfJohnDoe);
                dbContext.StorageLocations.Add(storageLocation);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        var expectedStorageLocationRequest = new StorageLocationRequest
        {
            Name = "Updated Location",
            Description = "Updated Description"
        };

        // Act
        var response = await httpClient.PutAsJsonAsync<StorageLocationRequest>($"api/v1/storage-locations/{storageLocation.StorageLocationId}", expectedStorageLocationRequest);

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.StorageLocations.Count().Should().Be(1);
            dbContext.StorageLocations.FirstOrDefault()!.StorageLocationId.Should().Be(1);
            dbContext.StorageLocations.FirstOrDefault()!.Name.Should().Be(expectedStorageLocationRequest.Name);
            dbContext.StorageLocations.FirstOrDefault()!.Description.Should().Be(expectedStorageLocationRequest.Description);
            dbContext.StorageLocations.FirstOrDefault()!.HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
        });
    }

    [Fact]
    public async Task DeleteStorageLocationAsync_ShouldWork()
    {
        // Arrange
        var storageLocation1 = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 1, Name = "Test Location", Description = "Bar Description" };
        var storageLocation2 = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 2, Name = "Unit Location", Description = "Foo Description" };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        testApplication.SetupDatabase<AppDbContext>(
            dbContext =>
            {
                dbContext.Accounts.Add(AccountJohnDoe);
                dbContext.Households.Add(HouseholdOfJohnDoe);
                dbContext.StorageLocations.Add(storageLocation1);
                dbContext.StorageLocations.Add(storageLocation2);
            });

        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        var response = await httpClient.DeleteAsync($"api/v1/storage-locations/{storageLocation1.StorageLocationId}");

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.StorageLocations.Count().Should().Be(1);
            dbContext.StorageLocations.FirstOrDefault()!.StorageLocationId.Should().Be(storageLocation2.StorageLocationId);
            dbContext.StorageLocations.FirstOrDefault()!.Name.Should().Be(storageLocation2.Name);
            dbContext.StorageLocations.FirstOrDefault()!.Description.Should().Be(storageLocation2.Description);
            dbContext.StorageLocations.FirstOrDefault()!.HouseholdId.Should().Be(HouseholdOfJohnDoe.HouseholdId);
        });
    }
}
