using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Pantry.Common.Logging.ConsoleFormatter;

internal sealed class ServiceRuntimeContractFormatter : Microsoft.Extensions.Logging.Console.ConsoleFormatter, IDisposable
{
    public const string FormatterName = nameof(ServiceRuntimeContractFormatter);

    private readonly IDisposable? _optionsReloadToken;

    private ServiceRuntimeContractFormatterOptions _formatterOptions = null!;

    private static readonly Action<object?, TextWriter> ProcessScope = (scope, state) =>
    {
        if (scope is IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            WriteStructuredValues(state, keyValuePairs);
        }
        else
        {
            state.Write(' ');
            state.Write(scope);
        }
    };

    public ServiceRuntimeContractFormatter(IOptionsMonitor<ServiceRuntimeContractFormatterOptions> options)
        : base(FormatterName)
    {
        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
    }

    public void Dispose()
    {
        _optionsReloadToken.Dispose();
    }

    /// <inheritdoc />
    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        // Example:
        // 2021-55-05 08:01:01.075 level=INFO eventId=2 eventName=RequestFinished sourceContext="Microsoft.AspNetCore.Hosting.Diagnostics"
        // message="Request finished HTTP/1.1 GET http://localhost/ - - - 200 - - 101.8717ms" elapsedMilliseconds="101.8717"
        // statusCode="200" contentType="" contentLength="" protocol="HTTP/1.1" method="GET" scheme="http" host="localhost" pathBase="" path="/"
        // queryString="" spanId="976b6fa436d88a4e" traceId="a23f2582c9522c42bce68662015ed2f1" parentId="0000000000000000"
        // requestId="0HM5HAUAVSI9J" requestPath="/"
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        if (logEntry.Exception == null && message == null)
        {
            return;
        }

        WriteTimestamp(textWriter);

        WriteLogLevel(textWriter, logEntry);

        WriteEventId(textWriter, logEntry);

        WriteEventName(textWriter, logEntry);

        WriteCategory(textWriter, logEntry);

        WriteMessage(textWriter, message);

        // The values which are passed together with the log entry
        // are substituted in the message and also logged as key-value-pair.
        if (_formatterOptions.IncludeParameters && logEntry.State is IEnumerable<KeyValuePair<string, object>> state)
        {
            WriteStructuredValues(textWriter, state);
        }

        if (_formatterOptions.IncludeScopes)
        {
            WriteScopeInformation(textWriter, scopeProvider);
        }

        if (_formatterOptions.IncludeBaggage)
        {
            WriteBaggage(textWriter);
        }

        if (_formatterOptions.IncludeTags)
        {
            WriteTags(textWriter);
        }

        // Example:
        // System.InvalidOperationException
        //    at Namespace.Class.Function() in File:line X
        if (logEntry.Exception != null)
        {
            WriteException(textWriter, logEntry.Exception);
        }

        textWriter.WriteLine();
    }

    private void ReloadLoggerOptions(ServiceRuntimeContractFormatterOptions options)
    {
        _formatterOptions = options;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteTimestamp(TextWriter textWriter)
    {
        var timestampFormat = _formatterOptions.TimestampFormat;
        var timestamp = _formatterOptions.UseUtcTimestamp
            ? DateTimeOffset.UtcNow.ToString(timestampFormat, CultureInfo.InvariantCulture)
            : DateTimeOffset.Now.ToString(timestampFormat, CultureInfo.InvariantCulture);
        textWriter.Write(timestamp);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteStructuredValues(TextWriter textWriter, IEnumerable<KeyValuePair<string, object>> keyValuePairs)
    {
        // Maybe some day this will come: https://github.com/eerhardt/runtime/commit/aa3345733a9fad585c70c05fad93f7b3d796528b
        foreach (KeyValuePair<string, object> pair in keyValuePairs)
        {
            if (!pair.Key.Equals("{OriginalFormat}", StringComparison.Ordinal))
            {
                WriteKeyValuePair(textWriter, pair.Key, pair.Value);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteCategory<TState>(TextWriter textWriter, LogEntry<TState> logEntry)
    {
        textWriter.Write(" sourceContext=\"");
        textWriter.Write(logEntry.Category);
        textWriter.Write('"');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteEventId<TState>(TextWriter textWriter, LogEntry<TState> logEntry)
    {
        // Check both because if the name is present, the 0 for the id is by intention.
        // This is e.g. for an incoming http request the case.
        if (logEntry.EventId.Id == 0 && string.IsNullOrEmpty(logEntry.EventId.Name))
        {
            return;
        }

        textWriter.Write(" eventId=");
        Span<char> span = stackalloc char[10]; // Int.Max is 2,147,483,647 and thus we know it has 10 digits.
        logEntry.EventId.Id.TryFormat(span, out var charsWritten, provider: CultureInfo.InvariantCulture);
        textWriter.Write(span[..charsWritten]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteEventName<TState>(TextWriter textWriter, LogEntry<TState> logEntry)
    {
        if (!string.IsNullOrEmpty(logEntry.EventId.Name))
        {
            textWriter.Write(" eventName=");
            textWriter.Write(logEntry.EventId.Name);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteLogLevel<TState>(TextWriter textWriter, LogEntry<TState> logEntry)
    {
        var logLevelString = GetLogLevelString(logEntry.LogLevel);
        textWriter.Write(' ');
        textWriter.Write(logLevelString);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteMessage(TextWriter textWriter, string? message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        textWriter.Write(" message=\"");
        textWriter.Write(message);
        textWriter.Write('"');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetLogLevelString(LogLevel logLevel)
    {
        const string prefix = "level=";

        return logLevel switch
        {
            LogLevel.Trace => prefix + "TRACE",
            LogLevel.Debug => prefix + "DEBUG",
            LogLevel.Information => prefix + "INFO",
            LogLevel.Warning => prefix + "WARN",
            LogLevel.Error => prefix + "ERROR",
            LogLevel.Critical => prefix + "FATAL",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider scopeProvider)
    {
        // The delegate is cached to prevent allocations.
        scopeProvider.ForEachScope(ProcessScope, textWriter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteTags(TextWriter textWriter)
    {
        if (Activity.Current != null)
        {
            WriteKeyValuePairs(textWriter, Activity.Current.TagObjects);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteBaggage(TextWriter textWriter)
    {
        if (Activity.Current != null)
        {
            WriteKeyValuePairs(textWriter, Activity.Current.Baggage);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteKeyValuePairs(TextWriter textWriter, IEnumerable<KeyValuePair<string, string?>> pairs)
    {
        foreach (KeyValuePair<string, string?> pair in pairs)
        {
            WriteKeyValuePair(textWriter, pair.Key, pair.Value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteKeyValuePairs(TextWriter textWriter, IEnumerable<KeyValuePair<string, object?>> pairs)
    {
        foreach (KeyValuePair<string, object?> pair in pairs)
        {
            WriteKeyValuePair(textWriter, pair.Key, pair.Value);
        }
    }

    private static void WriteKeyValuePair(TextWriter textWriter, string key, object? value)
    {
        textWriter.Write(' ');

        textWriter.Write(char.ToLower(key[0], CultureInfo.InvariantCulture));
        textWriter.Write(key.AsSpan()[1..key.Length]);

        textWriter.Write("=\"");
        if (value == null)
        {
            textWriter.Write(string.Empty);
        }
        else
        {
            var stringRepresentation = Convert.ToString(value, CultureInfo.InvariantCulture);
            textWriter.Write(stringRepresentation ?? string.Empty);
        }

        textWriter.Write('"');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteException(TextWriter textWriter, Exception exception)
    {
        textWriter.Write(" exception=\"");
        textWriter.Write(exception.ToString());
        textWriter.Write('"');
    }
}
