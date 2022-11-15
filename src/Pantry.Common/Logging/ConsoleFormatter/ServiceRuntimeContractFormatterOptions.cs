using System.Diagnostics;
using Microsoft.Extensions.Logging.Console;

namespace Pantry.Common.Logging.ConsoleFormatter;

/// <summary>
/// Options to configure the <see cref="ServiceRuntimeContractFormatter"/>.
/// </summary>
public sealed class ServiceRuntimeContractFormatterOptions : SimpleConsoleFormatterOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceRuntimeContractFormatterOptions"/> class.
    /// </summary>
    public ServiceRuntimeContractFormatterOptions()
    {
        IncludeScopes = true;
        SingleLine = true;
        TimestampFormat = "yyyy-MM-dd HH:mm:ss.fffzzz";
        UseUtcTimestamp = true;
    }

    /// <inheritdoc cref="SimpleConsoleFormatterOptions" />
    public new LoggerColorBehavior ColorBehavior
    {
        get => LoggerColorBehavior.Disabled;
        set => throw new NotSupportedException("This formatter does not support ColorBehavior.");
    }

    /// <summary>
    /// Specify if the parameter values are only logged as part of the message
    /// or appended to the log line as key-value-pair. Default is <c>true</c>.
    /// </summary>
    public bool IncludeParameters { get; set; } = true;

    /// <summary>
    /// Specify if <see cref="Activity.Tags"/> are logged as key-value-pairs.
    /// Default is <c>true</c>.
    /// </summary>
    public bool IncludeTags { get; set; } = true;

    /// <summary>
    /// Specify if <see cref="Activity.Baggage"/> are logged as key-value-pairs.
    /// Default is <c>true</c>.
    /// </summary>
    public bool IncludeBaggage { get; set; } = true;
}
