using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Pantry.Common.Diagnostics.HealthChecks;

internal static class JsonResponseProvider
{
    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Framework demand.")]
    public static async Task WriteJsonResponseAsync(HealthReport healthReport, bool includeDetails, Stream body)
    {
        var options = new JsonWriterOptions
        {
            Indented = false,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Do not encode e.g. "'"
            SkipValidation = true // For performance reasons.
        };

        await using var writer = new Utf8JsonWriter(body, options);

        writer.WriteStartObject();

        writer.WriteString("status", healthReport.Status.ToString("G"));
        writer.WriteString("totalDuration", healthReport.TotalDuration.ToString("g", CultureInfo.InvariantCulture));

        if (includeDetails)
        {
            writer.WriteStartObject("entries");

            foreach ((var entryKey, HealthReportEntry entryValue) in healthReport.Entries)
            {
                writer.WriteStartObject(entryKey);

                writer.WriteString("status", entryValue.Status.ToString("G"));

                if (!string.IsNullOrEmpty(entryValue.Description))
                {
                    writer.WriteString("description", entryValue.Description);
                }

                writer.WriteString("duration", entryValue.Duration.ToString("g", CultureInfo.InvariantCulture));

                if (entryValue.Exception != null)
                {
                    var exceptionValue = $"Exception of type '{entryValue.Exception.GetType().FullName}' was thrown.";
                    writer.WriteString("exception", exceptionValue);
                }

                writer.WriteStartObject("data");
                if (entryValue.Data?.Any() ?? false)
                {
                    foreach ((var dataKey, var dataValue) in entryValue.Data)
                    {
                        writer.WriteString(dataKey, dataValue?.ToString() ?? string.Empty);
                    }
                }

                writer.WriteEndObject(); // Close data object

                writer.WriteEndObject(); // Close entries object
            }

            writer.WriteEndObject(); // Close entries object
        }

        writer.WriteEndObject(); // Close Root object
    }
}
