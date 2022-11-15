using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Pantry.Service;
using Xunit;

namespace Pantry.Tests.Component.Integration.Service;

[Trait("Category", "Integration")]
public class ProgramFixture
{
    public static IEnumerable<object[]> GetEnvironments
    {
        get
        {
            yield return new object[] { Environments.Development };
            yield return new object[] { Pantry.Common.Hosting.Environments.IntegrationTest };
            yield return new object[] { Environments.Staging };
            yield return new object[] { Environments.Production };
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
        createWebHostBuilder.Should().NotThrow();
    }
}
