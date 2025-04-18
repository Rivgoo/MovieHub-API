namespace Domain.Abstractions;

public interface IBaseEntity<TId> : IEntity, IAuditableEntity
	where TId : notnull, IComparable<TId>
{
	TId Id { get; set; }
}