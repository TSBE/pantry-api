using System.Diagnostics.CodeAnalysis;
using Pantry.Common.EntityFrameworkCore.Exceptions;

namespace Microsoft.EntityFrameworkCore;

/// <summary>
///     Contains methods which extend the <see cref="DbSet{TEntity}"/>.
/// </summary>
public static class DbSetExtensions
{
    /// <summary>
    ///     Finds an entity with the given primary key values. If an entity with the given primary key values
    ///     is being tracked by the context, then it is returned immediately without making a request to the
    ///     database. Otherwise, a query is made to the database for an entity with the given primary key values
    ///     and this entity, if found, is attached to the context and returned. If no entity is found, then
    ///     a <see cref="EntityNotFoundException{TEntity}"/> is thrown.
    /// </summary>
    /// <param name="dbSet">The <see cref="DbSet{TEntity}"/> to execute the query on.</param>
    /// <param name="id">The primary key value.</param>
    /// <typeparam name="TSource">The type of the entity.</typeparam>
    /// <typeparam name="TId">The type of the entity primary key.</typeparam>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The entity found, or null.</returns>
    /// <exception cref="EntityNotFoundException">Thrown if the entity was not found.</exception>
    public static async Task<TSource> FindOrThrowAsync<TSource, TId>(this DbSet<TSource> dbSet, [NotNull] TId id, CancellationToken cancellationToken = default)
        where TSource : class
    {
        if (id == null)
        {
            throw new ArgumentException("The id cannot be null.", nameof(id));
        }

        TSource? entity = await dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity == null)
        {
            throw new EntityNotFoundException<TSource>(id);
        }

        return entity;
    }
}
