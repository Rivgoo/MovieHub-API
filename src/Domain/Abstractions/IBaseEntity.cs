using System.ComponentModel.DataAnnotations;

namespace Domain.Abstractions;

/// <summary>
/// Represents a fundamental entity within the domain that possesses a unique identifier and auditing properties.
/// </summary>
/// <typeparam name="TId">The type of the unique identifier for the entity.</typeparam>
/// <remarks>
/// This interface combines the concepts from <see cref="IEntity"/> and <see cref="IAuditableEntity"/>,
/// providing a common base for entities that require a primary key and automatic tracking
/// of creation and modification timestamps.
/// The type parameter <typeparamref name="TId"/> is constrained to be a non-nullable type
/// and must implement <see cref="IComparable{TId}"/> for comparison operations.
/// </remarks>
public interface IBaseEntity<TId> : IEntity, IAuditableEntity
	where TId : notnull, IComparable<TId>
{
	/// <summary>
	/// Gets or sets the unique identifier for the entity.
	/// </summary>
	/// <value>The unique identifier of the entity. This property typically serves as the primary key in persistence scenarios.</value>
	[Required]
	[Key]
	TId Id { get; set; }
}