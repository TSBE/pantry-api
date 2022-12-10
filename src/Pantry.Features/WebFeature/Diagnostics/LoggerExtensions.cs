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

    [LoggerMessage(
    EventId = 3,
    EventName = nameof(ExecutingOpenFoodFacts),
    Level = LogLevel.Information,
    Message = "Executing food fact search {GlobalTradeItemNumber}")]
    public static partial void ExecutingOpenFoodFacts(this ILogger logger, string globalTradeItemNumber);

    [LoggerMessage(
    EventId = 4,
    EventName = nameof(ExecutingEanSearchOrg),
    Level = LogLevel.Information,
    Message = "Executing ean search {GlobalTradeItemNumber}")]
    public static partial void ExecutingEanSearchOrg(this ILogger logger, string globalTradeItemNumber);

    [LoggerMessage(
    EventId = 5,
    EventName = nameof(ExecutedOpenFoodFacts),
    Level = LogLevel.Error,
    Message = "Open food fact search failed with exception {ExceptionMessage}")]
    public static partial void ExecutedOpenFoodFacts(this ILogger logger, string exceptionMessage);

    [LoggerMessage(
    EventId = 6,
    EventName = nameof(ExecutedEanSearchOrg),
    Level = LogLevel.Error,
    Message = "Ean product search failed with exception {ExceptionMessage}")]
    public static partial void ExecutedEanSearchOrg(this ILogger logger, string exceptionMessage);
}
