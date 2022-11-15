using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Pantry.Common.Diagnostics.HealthChecks;

internal class JsonResponseWriter
{
    private readonly bool _includeDetailsInResponse;

    public JsonResponseWriter(bool includeDetailsInResponse)
    {
        _includeDetailsInResponse = includeDetailsInResponse;
    }

    public async Task WriteResponse(HttpContext httpContext, HealthReport healthReport)
    {
        httpContext.Response.StatusCode = IsHealthyStatus(healthReport) ? (int)HttpStatusCode.OK : (int)HttpStatusCode.ServiceUnavailable;
        httpContext.Response.ContentType = "application/json; charset=utf-8";

        // To prevent heap allocations, the JSON is written directly to the stream.
        await JsonResponseProvider.WriteJsonResponseAsync(healthReport, _includeDetailsInResponse, httpContext.Response.Body);
    }

    private static bool IsHealthyStatus(HealthReport healthReport)
    {
        return healthReport.Status != HealthStatus.Unhealthy;
    }
}
