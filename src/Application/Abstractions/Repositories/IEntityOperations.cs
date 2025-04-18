using Domain.Abstractions;

namespace Application.Abstractions.Repositories;

/// <summary>
/// Defines a comprehensive contract for common CRUD and existence operations on a domain entity.
/// </summary>
/// <typeparam name="TEntity">The type of the entity managed by the repository.</typeparam>
/// <typeparam name="TId">The type of the unique identifier for the entity.</typeparam>
/// <remarks>
/// This interface combines fundamental repository capabilities:
/// <list type="bullet">
///   <item><see cref="IAddOperations{TEntity}"/>: Adding new entities.</item>
///   <item><see cref="IGetOperations{TEntity, TId}"/>: Retrieving entities by ID or all entities.</item>
///   <item><see cref="IUpdateOperations{TEntity}"/>: Updating existing entities.</item>
///   <item><see cref="IDeleteOperations{TEntity}"/>: Deleting entities.</item>
///   <item><see cref="IExistByIdOperation{TId}"/>: Checking entity existence by ID.</item>
/// </list>
/// It serves as a single point of access for standard entity persistence operations.
/// The type parameter <typeparamref name="TEntity"/> must implement <see cref="IEntity"/>.
/// The type parameter <typeparamref name="TId"/> must be a non-nullable type and implement <see cref="IComparable{TId}"/>.
/// </remarks>
public interface IEntityOperations<TEntity, TId> :
	IAddOperations<TEntity>,
	IGetOperations<TEntity, TId>,
	IUpdateOperations<TEntity>,
	IDeleteOperations<TEntity>,
	IExistByIdOperation<TId>
	where TEntity : IEntity
	where TId : notnull, IComparable<TId>
{
}