using System.ComponentModel.DataAnnotations;

namespace Domain.Abstractions;

/// <summary>
/// Provides a common abstract base class for domain entities with a generic identifier type and basic auditing properties.
/// </summary>
/// <typeparam name="TId">The type of the unique identifier for the entity.</typeparam>
/// <remarks>
/// This class implements the <see cref="IBaseEntity{TId}"/> interface and provides concrete implementations
/// for the <see cref="Id"/>, <see cref="CreatedAt"/>, and <see cref="UpdatedAt"/> properties.
/// It includes data annotations for persistence mapping with Entity Framework Core.
/// The <typeparamref name="TId"/> is constrained to be a non-nullable comparable type.
/// </remarks>
public abstract class BaseEntity<TId> : IBaseEntity<TId>
	where TId : notnull, IComparable<TId>
{
	/// <summary>
	/// Gets or sets the unique identifier for the entity.
	/// </summary>
	/// <value>
	/// The unique identifier of the entity. This property is decorated with
	/// <see cref="KeyAttribute"/> to denote it as the primary key,
	/// <see cref="RequiredAttribute"/> to enforce non-nullability at the database level,
	/// and the C# <see langword="required"/> keyword (C# 11+) to ensure it is
	/// initialized during object creation.
	/// </value>
	[Key]
	[Required]
	public required TId Id { get; set; }

	/// <summary>
	/// Gets or sets the date and time (typically UTC) when the entity was created.
	/// </summary>
	/// <value>
	/// The timestamp indicating the creation time. This field is required.
	/// Its value is often set automatically by application logic (e.g., in DbContext's SaveChanges).
	/// </value>
	[Required]
	public DateTime CreatedAt { get; set; }

	/// <summary>
	/// Gets or sets the date and time (typically UTC) when the entity was last updated.
	/// </summary>
	/// <value>
	/// The timestamp indicating the last update time. This field is required.
	/// Its value is often updated automatically by application logic (e.g., in DbContext's SaveChanges).
	/// </value>
	[Required]
	public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Provides a convenient abstract base class for domain entities with an integer identifier.
/// </summary>
/// <remarks>
/// This class inherits from <see cref="BaseEntity{TId}"/>, specifically using <see langword="int"/>
/// as the identifier type. It is intended as a shortcut for common cases where
/// an integer primary key is used, inheriting all properties (<see cref="Id"/>,
/// <see cref="CreatedAt"/>, <see cref="UpdatedAt"/>) and behaviors from the base class.
/// This class is abstract and should be inherited by concrete entity types.
/// </remarks>
public abstract class BaseEntity : BaseEntity<int>
{
}