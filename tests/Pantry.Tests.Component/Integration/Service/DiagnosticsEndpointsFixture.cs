using System.Net;

namespace Pantry.Tests.Component.Integration.Service;

[Trait("Category", "Integration")]
public class DiagnosticsEndpointsFixture
{
    private readonly ITestOutputHelper _testOutputHelper;

    public DiagnosticsEndpointsFixture(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData("health/ready")]
    [InlineData("health/live")]
    public async Task Endpoint_ShouldReturnOk(string endpoint)
    {
        // Arrange
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(_testOutputHelper);
        using HttpClient httpClient = testApplication.CreateClient();

        // Act
        HttpResponseMessage response = await httpClient.GetAsync(endpoint);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
