using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Pantry.Common.EntityFrameworkCore.Exceptions;

namespace Microsoft.EntityFrameworkCore;

/// <summary>
///     Contains methods which extend the <see cref="DbContext"/>.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    ///     Finds an entity with the given primary key values. If an entity with the given primary key values
    ///     is being tracked by the context, then it is returned immediately without making a request to the
    ///     database. Otherwise, a query is made to the database for an entity with the given primary key values
    ///     and this entity, if found, is attached to the context and returned. If no entity is found, then
    ///     a <see cref="EntityNotFoundException{TEntity}"/> is thrown.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/> the execute the action.</param>
    /// <param name="id">The primary key value.</param>
    /// <typeparam name="TSource">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the entity primary key.</typeparam>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The entity found, or null.</returns>
    /// <exception cref="EntityNotFoundException">Thrown if the entity was not found.</exception>
    public static async Task<TSource> FindOrThrowAsync<TSource, TId>(this DbContext dbContext, [NotNull] TId id, CancellationToken cancellationToken = default)
        where TSource : class
    {
        if (id == null)
        {
            throw new ArgumentException("The id cannot be null.", nameof(id));
        }

        var entity = await dbContext.FindAsync<TSource>(new object[] { id }, cancellationToken);
        if (entity == null)
        {
            throw new EntityNotFoundException<TSource>(id);
        }

        return entity;
    }

    /// <summary>
    ///     Asynchronously returns the first element of a sequence that satisfies a specified condition
    ///     or throws a <see cref="EntityNotFoundException"/> if no such element is found.
    /// </summary>
    /// <remarks>
    ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    ///     that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    ///     The type of the entity.
    /// </typeparam>
    /// <param name="dbContext">The db context to execute the action on to.</param>
    /// <param name="predicate"> A function to test each element for a condition. </param>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    ///     The task result contains the first element in <paramref name="dbContext" /> that passes the test specified by <paramref name="predicate" />.
    /// </returns>
    /// <exception cref="EntityNotFoundException">Thrown if the entity was not found.</exception>
    public static async Task<TSource> FirstOrThrowAsync<TSource>(
        [NotNull] this DbContext dbContext,
        [NotNull] Expression<Func<TSource, bool>> predicate,
        CancellationToken cancellationToken = default)
        where TSource : class
    {
        TSource? entity = await dbContext.Set<TSource>().FirstOrDefaultAsync(predicate, cancellationToken);
        if (entity == null)
        {
            throw new EntityNotFoundException<TSource>(predicate);
        }

        return entity;
    }
}
