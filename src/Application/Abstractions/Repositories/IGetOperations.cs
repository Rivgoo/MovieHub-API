using Domain.Abstractions;

namespace Application.Abstractions.Repositories;

/// <summary>
/// Defines a contract for repository operations that involve retrieving entities from the data store.
/// </summary>
/// <typeparam name="TEntity">The type of the entity managed by the repository.</typeparam>
/// <typeparam name="TId">The type of the unique identifier for the entity.</typeparam>
/// <remarks>
/// This interface provides methods for fetching single entities by identifier and retrieving all entities
/// in both synchronous and asynchronous manners.
/// Implementations typically use read-only queries (<c>.AsNoTracking()</c>) for performance.
/// The type parameter <typeparamref name="TEntity"/> must implement <see cref="IEntity"/>.
/// The type parameter <typeparamref name="TId"/> must implement <see cref="IComparable{TId}"/>.
/// </remarks>
public interface IGetOperations<TEntity, TId>
	where TEntity : IEntity
	where TId : IComparable<TId>
{
	/// <summary>
	/// Asynchronously retrieves all entities of the managed type from the data store.
	/// </summary>
	/// <remarks>
	/// This operation typically executes a query that returns all records for the entity type.
	/// It is usually configured to not track the retrieved entities for performance.
	/// </remarks>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>
	/// A task that represents the asynchronous operation.
	/// The task result contains an <see cref="ICollection{T}"/> of all entities.
	/// Returns an empty collection if no entities are found.
	/// </returns>
	Task<ICollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Synchronously retrieves all entities of the managed type from the data store.
	/// </summary>
	/// <remarks>
	/// This is a synchronous alternative to <see cref="GetAllAsync(CancellationToken)"/>.
	/// It is usually configured to not track the retrieved entities for performance.
	/// </remarks>
	/// <returns>
	/// An <see cref="ICollection{T}"/> of all entities.
	/// Returns an empty collection if no entities are found.
	/// </returns>
	ICollection<TEntity> GetAll();

	/// <summary>
	/// Synchronously retrieves an entity by its unique identifier from the data store.
	/// </summary>
	/// <remarks>
	/// This is a synchronous alternative to <see cref="GetByIdAsync(TId, CancellationToken)"/>.
	/// It is usually configured to not track the retrieved entity for performance.
	/// </remarks>
	/// <param name="id">The unique identifier of the entity to retrieve.</param>
	/// <returns>
	/// The entity with the specified identifier, or <see langword="null"/> if not found.
	/// </returns>
	TEntity? GetById(TId id);

	/// <summary>
	/// Asynchronously retrieves an entity by its unique identifier from the data store.
	/// </summary>
	/// <remarks>
	/// This operation typically executes a query that filters by the entity's primary key.
	/// It is usually configured to not track the retrieved entity for performance.
	/// </remarks>
	/// <param name="id">The unique identifier of the entity to retrieve.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>
	/// A task that represents the asynchronous operation.
	/// The task result contains the entity with the specified identifier, or <see langword="null"/> if not found.
	/// </returns>
	Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
}