namespace Application.Abstractions;

public interface IUnitOfWork
{
	void SaveChanges();
	Task SaveChangesAsync(CancellationToken cancellationToken = default);
}