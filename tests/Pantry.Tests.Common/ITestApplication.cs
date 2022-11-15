namespace Pantry.Tests.Common;

/// <summary>
///     Interface for TestApplications to allow access to common features.
/// </summary>
public interface ITestApplication
{
    /// <summary>
    ///     Gets the <see cref="IServiceProvider" /> used by this test application.
    /// </summary>
    IServiceProvider Services { get; }
}
