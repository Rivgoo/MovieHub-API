namespace Application.Abstractions.Repositories;

/// <summary>
/// Defines a contract for repository operations that check for the existence of an entity by its identifier.
/// </summary>
/// <typeparam name="TId">The type of the unique identifier for the entity.</typeparam>
/// <remarks>
/// This interface provides an asynchronous method to efficiently determine if an entity
/// with a specific identifier exists in the data store without loading the entire entity data.
/// The type parameter <typeparamref name="TId"/> must implement <see cref="IComparable{TId}"/>.
/// </remarks>
public interface IExistByIdOperation<TId> where TId : IComparable<TId>
{
	/// <summary>
	/// Asynchronously checks if an entity with the specified identifier exists in the data store.
	/// </summary>
	/// <remarks>
	/// This operation is typically optimized to avoid loading the full entity data,
	/// making it more efficient than retrieving the entity and checking for null.
	/// </remarks>
	/// <param name="entityId">The unique identifier of the entity to check.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>
	/// A task that represents the asynchronous operation.
	/// The task result is <see langword="true"/> if an entity with the given identifier exists; otherwise, <see langword="false"/>.
	/// </returns>
	Task<bool> ExistByIdAsync(TId entityId, CancellationToken cancellationToken = default);
}