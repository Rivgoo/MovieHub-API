using Domain.Abstractions;

namespace Application.Abstractions.Repositories;

/// <summary>
/// Defines a contract for repository operations that involve updating entities in the data store.
/// </summary>
/// <typeparam name="TEntity">The type of the entity managed by the repository.</typeparam>
/// <remarks>
/// Implementations of this interface are responsible for marking entities
/// as modified in the underlying data store's context. The actual persistence of changes
/// is typically handled by a separate Unit of Work.
/// The type parameter <typeparamref name="TEntity"/> must implement <see cref="IEntity"/>.
/// </remarks>
public interface IUpdateOperations<TEntity> where TEntity : IEntity
{
	/// <summary>
	/// Marks a single entity as modified to be updated in the data store upon saving changes.
	/// </summary>
	/// <remarks>
	/// This method typically attaches the entity to the change tracker (if detached)
	/// and sets its state to <see cref="Microsoft.EntityFrameworkCore.EntityState.Modified"/>.
	/// This may result in all properties being marked for update depending on the implementation.
	/// </remarks>
	/// <param name="entity">The entity object with updated values.</param>
	void Update(TEntity entity);

	/// <summary>
	/// Marks a collection of entities as modified to be updated in the data store upon saving changes.
	/// </summary>
	/// <remarks>
	/// This method typically attaches the entities to the change tracker (if detached)
	/// and sets their state to <see cref="Microsoft.EntityFrameworkCore.EntityState.Modified"/>.
	/// This may result in all properties being marked for update for each entity depending on the implementation.
	/// </remarks>
	/// <param name="entities">The collection of entity objects with updated values.</param>
	void UpdateRange(ICollection<TEntity> entities);
}