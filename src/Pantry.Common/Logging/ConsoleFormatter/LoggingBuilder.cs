using Microsoft.Extensions.Logging.Console;
using Pantry.Common.Logging.ConsoleFormatter;

// This method should be visible when using Microsoft.Extensions.Logging.
namespace Microsoft.Extensions.Logging;

/// <summary>
/// Extension methods for the <see cref="ILoggingBuilder"/>.
/// </summary>
public static class LoggingBuilder
{
    /// <summary>
    /// Add console logging with a <see cref="ConsoleFormatter"/> which implements the custom service runtime contract for logging.
    /// </summary>
    /// <returns>The builder.</returns>
    public static ILoggingBuilder AddPantryConsole(this ILoggingBuilder builder, Action<ServiceRuntimeContractFormatterOptions>? configure = null)
    {
        builder.AddConsole(options => options.FormatterName = ServiceRuntimeContractFormatter.FormatterName);
        if (configure == null)
        {
            builder.AddConsoleFormatter<ServiceRuntimeContractFormatter, ServiceRuntimeContractFormatterOptions>();
        }
        else
        {
            builder.AddConsoleFormatter<ServiceRuntimeContractFormatter, ServiceRuntimeContractFormatterOptions>(configure);
        }

        return builder;
    }
}
