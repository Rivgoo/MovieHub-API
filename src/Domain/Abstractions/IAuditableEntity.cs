namespace Domain.Abstractions;

/// <summary>
/// Represents an auditable entity with created and updated timestamps.
/// </summary>
public interface IAuditableEntity
{
	/// <summary>
	/// Gets or sets the date and time when the entity was created.
	/// </summary>
	DateTime CreatedAt { get; set; }

	/// <summary>
	/// Gets or sets the date and time when the entity was last updated.
	/// </summary>
	DateTime UpdatedAt { get; set; }
}