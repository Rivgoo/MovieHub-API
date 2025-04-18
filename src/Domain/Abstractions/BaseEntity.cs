using System.ComponentModel.DataAnnotations;

namespace Domain.Abstractions;

public abstract class BaseEntity<TId> : IBaseEntity<TId> 
	where TId : notnull, IComparable<TId>
{
	[Key]
	[Required]
	public required TId Id { get; set; }

	[Required]
	public DateTime CreatedAt { get; set; }

	[Required]
	public DateTime UpdatedAt { get; set; }
}

public abstract class BaseEntity : BaseEntity<int>
{
}