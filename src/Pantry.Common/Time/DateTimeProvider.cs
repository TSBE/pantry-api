namespace Pantry.Common.Time;

/// <summary>
/// Provides date and time information.
/// <remarks>
/// This class is a testable wrapper around the native <see cref="DateTime"/> functions.
/// When used in a <see cref="DateTimeContext"/> it returns the date and time values
/// that are configured in that context.
/// If an instance of this class is used outside of a <see cref="DateTimeContext"/> it
/// returns the real date and time values.
/// </remarks>
/// </summary>
public static class DateTimeProvider
{
    private static readonly Func<DateTime> DefaultNowFunction = () => DateTime.Now;
    private static readonly Func<DateTime> DefaultTodayFunction = () => DateTime.Today;
    private static readonly Func<DateTime> DefaultUtcNowFunction = () => DateTime.UtcNow;

    /// <summary>
    /// Returns the current date and time (local time).
    /// If this is called within a <see cref="DateTimeContext"/> it
    /// returns the value that is valid in that context.
    /// </summary>
    public static DateTime Now => DateTimeContext.Current == null
        ? DefaultNowFunction.Invoke()
        : DateTimeContext.Current.ContextDateTimeNow;

    /// <summary>
    /// Returns the date part of the current datetime (local time).
    /// If this is called within a <see cref="DateTimeContext"/> it
    /// returns the value that is valid in that context.
    /// </summary>
    public static DateTime Today => DateTimeContext.Current == null
        ? DefaultTodayFunction.Invoke()
        : DateTimeContext.Current.ContextDateTimeToday;

    /// <summary>
    /// Returns the current UTC datetime value.
    /// If this is called within a <see cref="DateTimeContext"/> it
    /// returns the value that is valid in that context.
    /// </summary>
    public static DateTime UtcNow => DateTimeContext.Current == null
        ? DefaultUtcNowFunction.Invoke()
        : DateTimeContext.Current.ContextDateTimeUtcNow;
}
