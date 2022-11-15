using FluentAssertions;
using Xunit;

namespace Pantry.Tests.SystemIntegration;

[Trait("Category", "SystemIntegration")]
public class ExampleFeature
{
    [Fact]
    public void Execute_ShouldWork()
    {
        // Code here.
        true.Should().BeTrue();
    }
}
