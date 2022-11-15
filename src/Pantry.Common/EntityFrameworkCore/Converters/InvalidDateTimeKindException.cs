using System.Runtime.Serialization;

namespace Pantry.Common.EntityFrameworkCore.Converters;

/// <summary>
///     This Exception is thrown if the <see cref="DateTimeKindConverter" /> is used and a DateTime Field has
///     a <see cref="DateTimeKind" /> not equal to UTC.
/// </summary>
[Serializable]
public class InvalidDateTimeKindException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="InvalidDateTimeKindException" /> class.
    /// </summary>
    public InvalidDateTimeKindException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="InvalidDateTimeKindException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public InvalidDateTimeKindException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="InvalidDateTimeKindException" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public InvalidDateTimeKindException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="InvalidDateTimeKindException" /> class.
    /// </summary>
    /// <param name="serializationInfo">The serializationInfo.</param>
    /// <param name="streamingContext">The streamingContext.</param>
    protected InvalidDateTimeKindException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base(serializationInfo, streamingContext)
    {
        throw new NotImplementedException();
    }
}
