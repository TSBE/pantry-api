namespace Pantry.Common.Time;

/// <summary>
/// Represents a context within which a fixed date and time are defined.
/// When a <see cref="DateTimeProvider"/> instance is used within such
/// a context that provider returns the defined datetime value
/// as the current datetime.
/// </summary>
public class DateTimeContext : IDisposable
{
    /// <summary>
    /// The usage of this stack allows nesting of <see cref="DateTimeContext"/>s. By using
    /// an <see cref="AsyncLocal{T}"/> those values are local to each async context (Threads or async method calls).
    /// </summary>
    private static readonly AsyncLocal<Stack<DateTimeContext>?> DateTimeContexts = new();

    private static readonly object SyncRoot = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeContext"/> class.
    /// </summary>
    /// <param name="contextDateTimeUtcNow">The UTC datetime value that is valid in this context.</param>
    public DateTimeContext(DateTime contextDateTimeUtcNow)
    {
        if (DateTimeContexts.Value == null)
        {
            lock (SyncRoot)
            {
                DateTimeContexts.Value ??= new Stack<DateTimeContext>();
            }
        }

        ContextDateTimeUtcNow = contextDateTimeUtcNow.ToUniversalTime();
        DateTimeContexts.Value.Push(this);
    }

    /// <summary>
    /// The current UTC datetime value of this context.
    /// </summary>
    public DateTime ContextDateTimeUtcNow { get; }

    /// <summary>
    /// The current datetime value of this context (local time).
    /// </summary>
    public DateTime ContextDateTimeNow => ContextDateTimeUtcNow.ToLocalTime();

    /// <summary>
    /// The date part of the current datetime value of this context (local time).
    /// </summary>
    public DateTime ContextDateTimeToday => ContextDateTimeNow.Date;

    /// <summary>
    /// The current UTC datetime value of this context.
    /// </summary>
    public DateTimeOffset ContextDateTimeOffsetUtcNow => ContextDateTimeUtcNow;

    /// <summary>
    /// The current datetime value of this context (local time).
    /// </summary>
    public DateTimeOffset ContextDateTimeOffsetNow => ContextDateTimeUtcNow.ToLocalTime();

    /// <summary>
    /// The current DateTimeContext.
    /// <returns>The current <see cref="DateTimeContext"/> if there is one active. Else <c>null</c>. </returns>
    /// </summary>
    public static DateTimeContext? Current =>
        DateTimeContexts.Value != null && DateTimeContexts.Value.TryPeek(out DateTimeContext? currentContext)
            ? currentContext : null;

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose this instance.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DateTimeContexts.Value?.Pop();
        }
    }
}
