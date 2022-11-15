namespace Pantry.Common.Hosting;

/// <summary>
///     Commonly used environment names, in addition to <see cref="Microsoft.Extensions.Hosting.Environments" />.
/// </summary>
public static class Environments
{
    /// <summary>
    ///     The environment used to run the integration tests.
    /// </summary>
    public const string IntegrationTest = nameof(IntegrationTest);
}
