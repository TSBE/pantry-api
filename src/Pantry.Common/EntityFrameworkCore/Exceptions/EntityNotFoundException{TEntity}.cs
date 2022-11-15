using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Pantry.Common.EntityFrameworkCore.Exceptions;

/// <inheritdoc />
[Serializable]
public class EntityNotFoundException<TEntity> : EntityNotFoundException
{
    /// <inheritdoc cref="EntityNotFoundException()" />
    public EntityNotFoundException()
    {
    }

    /// <inheritdoc cref="EntityNotFoundException(string, object)" />
    public EntityNotFoundException(object primaryKey)
        : base(typeof(TEntity).Name, primaryKey)
    {
    }

    /// <inheritdoc cref="EntityNotFoundException(string, Expression)" />
    public EntityNotFoundException(Expression searchExpression)
        : base(typeof(TEntity).Name, searchExpression)
    {
    }

    /// <inheritdoc cref="EntityNotFoundException(string?)" />
    public EntityNotFoundException(string? message)
        : base(message)
    {
    }

    /// <inheritdoc cref="EntityNotFoundException(string?, Exception?)" />
    public EntityNotFoundException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <inheritdoc cref="EntityNotFoundException(SerializationInfo, StreamingContext)" />
    protected EntityNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
