using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Pantry.Tests.Common;

/// <summary>
///     Factory for bootstrapping an application in memory for integration tests.
/// </summary>
/// <typeparam name="TStartup">
///     A type in the entry point assembly of the application.
///     Typically the Startup or Program classes can be used.
/// </typeparam>
public abstract class IntegrationTestWebApplicationFactoryBase<TStartup> : WebApplicationFactory<TStartup>, ITestApplication
    where TStartup : class
{
    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Pantry.Common.Hosting.Environments.IntegrationTest);
    }
}
