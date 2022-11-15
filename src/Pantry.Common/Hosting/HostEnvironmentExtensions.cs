namespace Microsoft.Extensions.Hosting;

/// <summary>
///     Provides Extensions for the <see cref="IHostEnvironment" />.
/// </summary>
public static class HostEnvironmentExtensions
{
    /// <summary>
    ///     Checks if the current hosting environment name is
    ///     <see cref="Pantry.Common.Hosting.Environments.IntegrationTest" />.
    /// </summary>
    /// <param name="hostEnvironment">An instance of <see cref="IHostEnvironment" />.</param>
    /// <returns>
    ///     True if the environment name is <see cref="Pantry.Common.Hosting.Environments.IntegrationTest" />,
    ///     otherwise false.
    /// </returns>
    public static bool IsIntegrationTest(this IHostEnvironment hostEnvironment) =>
        hostEnvironment.IsEnvironment(Pantry.Common.Hosting.Environments.IntegrationTest);
}
