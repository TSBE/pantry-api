using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Pantry.Common.Diagnostics.HealthChecks;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extensions for <see cref="IEndpointRouteBuilder"/>.
/// </summary>
public static class EndpointConventionBuilderExtensions
{
    /// <summary>
    ///     Registers three healthCheck endpoints to confirm the service runtime contract.
    ///     The readiness check uses all registered checks with the 'ready'-tag (
    ///     <see cref="HealthCheckConstants.Endpoints.ReadinessProbeUrl" />).
    ///     The startup check uses all registered checks with the 'startup'-tag (
    ///     <see cref="HealthCheckConstants.Endpoints.StartupProbeUrl" />).
    ///     The liveness filters out all checks and just returns success.
    /// </summary>
    /// <param name="endpoints">
    ///     The <see cref="IEndpointRouteBuilder" /> to add the health checks endpoint to.
    /// </param>
    /// <param name="includeDetailsInResponse">
    ///     A flag to indicate whether to include HealthCheck result details as JSON in the response.
    /// </param>
    /// <param name="customizationAction">
    ///    An optional delegate that can be used to customize the endpoint convention.
    /// </param>
    /// <returns>The <see cref="IEndpointRouteBuilder" /> so that additional calls can be chained.</returns>
    public static IEndpointRouteBuilder MapPantryHealthChecks(
        this IEndpointRouteBuilder endpoints,
        bool includeDetailsInResponse,
        Action<IEndpointConventionBuilder>? customizationAction)
    {
        if (endpoints == null)
        {
            throw new ArgumentNullException(nameof(endpoints));
        }

        var responseWriter = new JsonResponseWriter(includeDetailsInResponse);

        customizationAction ??= _ => { };

        customizationAction.Invoke(
            endpoints.MapHealthChecks(HealthCheckConstants.Endpoints.ReadinessProbeUrl, new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains(HealthCheckConstants.Tags.ReadinessTag),
                ResponseWriter = responseWriter.WriteResponse,
                AllowCachingResponses = false
            }));
        customizationAction.Invoke(
            endpoints.MapHealthChecks(HealthCheckConstants.Endpoints.LivenessProbeUrl, new HealthCheckOptions
            {
                Predicate = check => false,
                ResponseWriter = responseWriter.WriteResponse,
                AllowCachingResponses = false
            }));
        customizationAction.Invoke(
            endpoints.MapHealthChecks(HealthCheckConstants.Endpoints.StartupProbeUrl, new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains(HealthCheckConstants.Tags.StartupTag),
                ResponseWriter = responseWriter.WriteResponse,
                AllowCachingResponses = false
            }));

        return endpoints;
    }
}
