namespace Pantry.Tests.Component.Integration.Service;

[Trait("Category", "Integration")]
public class SwaggerFixture
{
    private readonly ITestOutputHelper _testOutputHelper;

    public SwaggerFixture(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Swagger_ShouldBeAccessible()
    {
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(_testOutputHelper);
        using HttpClient httpClient = testApplication.CreateClient();

        HttpResponseMessage response = await httpClient.GetAsync("api/doc/ui/index.html");

        response.ShouldNotBeNull();
        response.IsSuccessStatusCode.ShouldBeTrue();
    }

    [Fact]
    public async Task SwaggerJson_ShouldBeAccessible()
    {
        await using IntegrationTestWebApplicationFactory testApplication = await IntegrationTestWebApplicationFactory.CreateAsync(_testOutputHelper);
        using HttpClient client = testApplication.CreateClient();

        HttpResponseMessage response = await client.GetAsync("api/doc/v1.json");

        response.ShouldNotBeNull();
        response.IsSuccessStatusCode.ShouldBeTrue();
    }
}
