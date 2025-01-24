using Pantry.Core.Persistence;
using Pantry.Core.Persistence.Entities;
using Pantry.Features.WebFeature.V1.Controllers.Requests;
using Pantry.Features.WebFeature.V1.Controllers.Responses;

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
        await testApplication.SetupDatabaseAsync<AppDbContext>(
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
        response!.StorageLocations?.Count().ShouldBe(2);
        response.StorageLocations!.First().Id.ShouldBe(storageLocation1.StorageLocationId);
    }

    [Fact]
    public async Task GetStorageLocationByIdAsync_ShouldReturnStorageLocation()
    {
        // Arrange
        var storageLocation1 = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 1, Name = "Test Location", Description = "Bar Description" };
        var storageLocation2 = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 2, Name = "Unit Location", Description = "Foo Description" };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
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
        response!.Id.ShouldBe(storageLocation1.StorageLocationId);
    }

    [Fact]
    public async Task PostStorageLocationAsync_ShouldReturnStorageLocation()
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
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
        var response = await httpClient.PostAsJsonAsync("api/v1/storage-locations", expectedStorageLocationRequest);

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.StorageLocations.Count().ShouldBe(1);
            dbContext.StorageLocations.FirstOrDefault()!.StorageLocationId.ShouldBe(1);
            dbContext.StorageLocations.FirstOrDefault()!.Name.ShouldBe(expectedStorageLocationRequest.Name);
            dbContext.StorageLocations.FirstOrDefault()!.Description.ShouldBe(expectedStorageLocationRequest.Description);
            dbContext.StorageLocations.FirstOrDefault()!.HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
        });
    }

    [Fact]
    public async Task PutStorageLocationAsync_ShouldReturnStorageLocation()
    {
        // Arrange
        var storageLocation = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 1, Name = "Test Location", Description = "Bar Description" };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
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
        var response = await httpClient.PutAsJsonAsync($"api/v1/storage-locations/{storageLocation.StorageLocationId}", expectedStorageLocationRequest);

        // Assert
        response.EnsureSuccessStatusCode();

        testApplication.AssertDatabaseContent<AppDbContext>(dbContext =>
        {
            dbContext.StorageLocations.Count().ShouldBe(1);
            dbContext.StorageLocations.FirstOrDefault()!.StorageLocationId.ShouldBe(1);
            dbContext.StorageLocations.FirstOrDefault()!.Name.ShouldBe(expectedStorageLocationRequest.Name);
            dbContext.StorageLocations.FirstOrDefault()!.Description.ShouldBe(expectedStorageLocationRequest.Description);
            dbContext.StorageLocations.FirstOrDefault()!.HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
        });
    }

    [Fact]
    public async Task DeleteStorageLocationAsync_ShouldWork()
    {
        // Arrange
        var storageLocation1 = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 1, Name = "Test Location", Description = "Bar Description" };
        var storageLocation2 = new StorageLocation { Household = HouseholdOfJohnDoe, StorageLocationId = 2, Name = "Unit Location", Description = "Foo Description" };
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(TestOutputHelper);
        await testApplication.SetupDatabaseAsync<AppDbContext>(
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
            dbContext.StorageLocations.Count().ShouldBe(1);
            dbContext.StorageLocations.FirstOrDefault()!.StorageLocationId.ShouldBe(storageLocation2.StorageLocationId);
            dbContext.StorageLocations.FirstOrDefault()!.Name.ShouldBe(storageLocation2.Name);
            dbContext.StorageLocations.FirstOrDefault()!.Description.ShouldBe(storageLocation2.Description);
            dbContext.StorageLocations.FirstOrDefault()!.HouseholdId.ShouldBe(HouseholdOfJohnDoe.HouseholdId);
        });
    }
}
