using Microsoft.Extensions.Logging;

namespace Pantry.Features.WebFeature.Diagnostics;

public static partial class LoggerExtensions
{
    [LoggerMessage(
        EventId = 1,
        EventName = "ExecutingCommand",
        Level = LogLevel.Information,
        Message = "Executing command {CommandName}")]
    public static partial void ExecutingCommand(this ILogger logger, string commandName);

    [LoggerMessage(
        EventId = 2,
        EventName = "ExecutingQuery",
        Level = LogLevel.Information,
        Message = "Executing query {QueryName}")]
    public static partial void ExecutingQuery(this ILogger logger, string queryName);
}
