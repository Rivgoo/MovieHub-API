using Application.Results;
using Domain.Abstractions;

namespace Application.Abstractions.Services;

/// <summary>
/// Defines a generic service contract for managing domain entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity the service manages.</typeparam>
/// <typeparam name="TId">The type of the entity's unique identifier.</typeparam>
/// <remarks>
/// This interface provides standard CRUD (Create, Read, Update, Delete) operations
/// and existence checks for entities within the application layer.
/// The type parameter <typeparamref name="TEntity"/> must inherit from <see cref="BaseEntity{TId}"/>,
/// and <typeparamref name="TId"/> must implement <see cref="IComparable{TId}"/>.
/// Operations return a <see cref="Result{TValue}"/> or <see cref="Result"/>
/// to indicate success or failure, including associated errors.
/// </remarks>
public interface IEntityService<TEntity, TId>
	where TEntity : IBaseEntity<TId>
	where TId : IComparable<TId>
{
	/// <summary>
	/// Asynchronously retrieves an entity by its unique identifier.
	/// </summary>
	/// <param name="entityId">The unique identifier of the entity.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>
	/// A task that represents the asynchronous operation.
	/// The task result contains a <see cref="Result{TValue}"/> of type <typeparamref name="TEntity"/>.
	/// It will be <see cref="Result.Ok()"/> with the entity if found, or <see cref="Result.Bad(Error)"/>
	/// with a <see cref="EntityErrors{TEntity, TId}.NotFound(TId)"/> error if not found.
	/// </returns>
	Task<Result<TEntity>> GetByIdAsync(TId entityId, CancellationToken cancellationToken = default);

	/// <summary>
	/// Asynchronously retrieves all entities of the managed type.
	/// </summary>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>
	/// A task that represents the asynchronous operation.
	/// The task result contains an <see cref="ICollection{T}"/> of all entities.
	/// Returns an empty collection if no entities are found.
	/// </returns>
	Task<ICollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Asynchronously checks if an entity with the specified identifier exists.
	/// </summary>
	/// <param name="entityId">The unique identifier of the entity to check. Can be null.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>
	/// A task that represents the asynchronous operation.
	/// The task result is <see langword="true"/> if the entity exists; otherwise, <see langword="false"/>.
	/// Returns <see langword="false"/> if <paramref name="entityId"/> is null.
	/// </returns>
	Task<bool> ExistsByIdAsync(TId? entityId, CancellationToken cancellationToken = default);

	/// <summary>
	/// Asynchronously verifies that an entity with the specified identifier exists.
	/// </summary>
	/// <remarks>
	/// This method is useful for scenarios where an entity is expected to exist,
	/// and a specific error should be returned if it does not.
	/// </remarks>
	/// <param name="entityId">The unique identifier of the entity to verify. Can be null.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>
	/// A task that represents the asynchronous operation.
	/// The task result contains a <see cref="Result"/>.
	/// It will be <see cref="Result.Ok()"/> if the entity exists, or <see cref="Result.Bad(Error)"/>
	/// with a <see cref="EntityErrors{TEntity, TId}.NotFound(TId)"/> error if the entity does not exist or <paramref name="entityId"/> is null.
	/// </returns>
	Task<Result> VerifyExistsByIdAsync(TId? entityId, CancellationToken cancellationToken = default);

	/// <summary>
	/// Asynchronously creates a new entity in the data store.
	/// </summary>
	/// <param name="newEntity">The entity object to create.</param>
	/// <returns>
	/// A task that represents the asynchronous operation.
	/// The task result contains a <see cref="Result{TValue}"/> of type <typeparamref name="TEntity"/>.
	/// It will be <see cref="Result.Ok()"/> with the created entity if successful,
	/// or <see cref="Result.Bad(Error)"/> if the entity is null or validation fails.
	/// </returns>
	Task<Result<TEntity>> CreateAsync(TEntity newEntity);

	/// <summary>
	/// Asynchronously updates an existing entity in the data store.
	/// </summary>
	/// <param name="changedEntity">The entity object with updated values. The <see cref="BaseEntity{TId}.Id"/> property must be set.</param>
	/// <returns>
	/// A task that represents the asynchronous operation.
	/// The task result contains a <see cref="Result{TValue}"/> of type <typeparamref name="TEntity"/>.
	/// It will be <see cref="Result.Ok()"/> with the updated entity if successful,
	/// or <see cref="Result.Bad(Error)"/> if the entity is null, validation fails, or the entity does not exist.
	/// </returns>
	Task<Result<TEntity>> UpdateAsync(TEntity changedEntity);

	/// <summary>
	/// Asynchronously deletes an entity by its unique identifier from the data store.
	/// </summary>
	/// <param name="entityId">The unique identifier of the entity to delete.</param>
	/// <returns>
	/// A task that represents the asynchronous operation.
	/// The task result contains a <see cref="Result"/>.
	/// It will be <see cref="Result.Ok()"/> if the deletion is successful,
	/// or <see cref="Result.Bad(Error)"/> if the entity is not found or deletion fails.
	/// </returns>
	Task<Result> DeleteByIdAsync(TId entityId);
}