namespace Application.Abstractions.Repositories;

public interface IExistByIdOperation<TId> where TId : IComparable<TId>
{
	Task<bool> ExistByIdAsync(TId entityId, CancellationToken cancellationToken = default);
}