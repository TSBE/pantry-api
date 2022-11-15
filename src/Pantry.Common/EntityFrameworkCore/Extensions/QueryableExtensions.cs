using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Pantry.Common.EntityFrameworkCore.Exceptions;

namespace Microsoft.EntityFrameworkCore;

/// <summary>
///     Contains methods which extend the <see cref="IQueryable{TEntity}"/>.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    ///     Asynchronously returns the first element of a sequence that satisfies a specified condition
    ///     or throws a <see cref="EntityNotFoundException"/> if no such element is found.
    /// </summary>
    /// <remarks>
    ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
    ///     that any asynchronous operations have completed before calling another method on this context.
    /// </remarks>
    /// <typeparam name="TSource">
    ///     The type of the elements of <paramref name="source" />.
    /// </typeparam>
    /// <param name="source">
    ///     An <see cref="IQueryable{T}" /> to return the first element of.
    /// </param>
    /// <param name="predicate"> A function to test each element for a condition. </param>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    ///     The task result contains the first element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.
    /// </returns>
    /// <exception cref="EntityNotFoundException">Thrown if the entity was not found.</exception>
    public static async Task<TSource> FirstOrThrowAsync<TSource>(
        [NotNull] this IQueryable<TSource> source,
        [NotNull] Expression<Func<TSource, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        TSource? entity = await source.FirstOrDefaultAsync(predicate, cancellationToken);
        if (entity == null)
        {
            throw new EntityNotFoundException<TSource>(predicate);
        }

        return entity;
    }
}
