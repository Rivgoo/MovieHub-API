using Domain.Abstractions;

namespace Application.Abstractions.Repositories;

/// <summary>
/// Defines a contract for repository operations that involve deleting entities from the data store.
/// </summary>
/// <typeparam name="TEntity">The type of the entity managed by the repository.</typeparam>
/// <remarks>
/// Implementations of this interface are responsible for marking entities
/// to be removed from the underlying data store. The actual persistence of changes
/// is typically handled by a separate Unit of Work.
/// The type parameter <typeparamref name="TEntity"/> must implement <see cref="IEntity"/>.
/// </remarks>
public interface IDeleteOperations<TEntity> where TEntity : IEntity
{
	/// <summary>
	/// Marks a single entity to be removed from the data store upon saving changes.
	/// </summary>
	/// <param name="entity">The entity object to remove.</param>
	void Remove(TEntity entity);
}