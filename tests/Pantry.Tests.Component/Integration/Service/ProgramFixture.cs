using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Pantry.Service;

namespace Pantry.Tests.Component.Integration.Service;

[Trait("Category", "Integration")]
public class ProgramFixture
{
    public static IEnumerable<object[]> GetEnvironments
    {
        get
        {
            yield return [Environments.Development];
            yield return [Pantry.Common.Hosting.Environments.IntegrationTest];
            yield return [Environments.Staging];
            yield return [Environments.Production];
        }
    }

    [Theory]
    [MemberData(nameof(GetEnvironments))]
    public void CreateWebHostBuilder_Should_BeSuccessful(string environment)
    {
        Action createWebHostBuilder = () =>
        {
            IHostBuilder hostBuilder = Program.CreateHostBuilder(Array.Empty<string>());
            hostBuilder.UseEnvironment(environment);
            hostBuilder.Build();
        };
        createWebHostBuilder.ShouldNotThrow();
    }
}
