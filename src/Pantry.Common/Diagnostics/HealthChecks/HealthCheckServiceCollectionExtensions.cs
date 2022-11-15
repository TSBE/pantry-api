using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>.
/// </summary>
public static class HealthCheckServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="HealthCheckService"/> to the service collection.
    /// <remarks>
    ///     This library used to contain some graceful shutdown utilities for Kubernetes
    ///     that are not necessary anymore.
    ///     For compatibility reasons this method is kept in the library but it actually
    ///     does nothing.
    /// </remarks>
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
    public static IHealthChecksBuilder AddPantryHealthChecks(this IServiceCollection services)
    {
        IHealthChecksBuilder healthChecksBuilder = services.AddHealthChecks();

        return healthChecksBuilder;
    }
}
