using Xunit.Abstractions;

namespace Pantry.Tests.Component.Integration.Controllers;

public abstract class BaseControllerFixture
{
    protected BaseControllerFixture(ITestOutputHelper testOutputHelper)
    {
        TestOutputHelper = testOutputHelper;
    }

    protected ITestOutputHelper TestOutputHelper { get; }
}
