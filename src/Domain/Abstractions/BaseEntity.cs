using System.ComponentModel.DataAnnotations;

namespace Domain.Abstractions;

public abstract class BaseEntity<TId> : IEntity where TId : notnull, IComparable<TId>
{
	[Key]
	[Required]
	public required TId Id { get; set; }
}

public abstract class BaseEntity : BaseEntity<int>
{
}