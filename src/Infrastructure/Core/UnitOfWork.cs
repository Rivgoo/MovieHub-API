using Application.Abstractions;

namespace Infrastructure.Core;

internal sealed class UnitOfWork(CoreDbContext dbContext) : IUnitOfWork
{
	private readonly CoreDbContext _dbContext = dbContext;

	public void SaveChanges() => _dbContext.SaveChanges();
	public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
		=> await _dbContext.SaveChangesAsync(cancellationToken);
}