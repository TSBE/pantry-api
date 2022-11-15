namespace Pantry.Common.Diagnostics.HealthChecks;

/// <summary>
///     Defines common constants which are used for health checks.
///     As there are a lot of strings to be used, they should be encapsulated.
/// </summary>
public static class HealthCheckConstants
{
    /// <summary>
    ///     Defines the possible tags to be assigned to health checks.
    ///     Use tags to group health checks by their purpose.
    /// </summary>
    public static class Tags
    {
        /// <summary>
        ///     Tags to be assigned to health checks which are executed for a readiness probe.
        /// </summary>
        public const string ReadinessTag = "ready";

        /// <summary>
        ///     Tags to be assigned to health checks which are executed for a liveness probe.
        /// </summary>
        public const string LivenessTag = "live";

        /// <summary>
        ///     Tags to be assigned to health checks which are executed for a startup probe.
        /// </summary>
        public const string StartupTag = "startup";
    }

    /// <summary>
    ///     Defines the endpoints which exposes the health checks.
    /// </summary>
    public static class Endpoints
    {
        /// <summary>
        ///     The endpoint to probe for the readiness check.
        ///     This endpoint should return 200, if the application is ready to serve requests.
        /// </summary>
        public const string ReadinessProbeUrl = "/health/ready";

        /// <summary>
        ///     The endpoint to probe for the live check.
        ///     This endpoint should return 200, if the application is up and running.
        /// </summary>
        public const string LivenessProbeUrl = "/health/live";

        /// <summary>
        ///     The endpoint to probe for the startup check.
        ///     As soon as status 200 is being returned, readiness and liveness take over and will be probed periodically.
        /// </summary>
        public const string StartupProbeUrl = "/health/startup";
    }
}
