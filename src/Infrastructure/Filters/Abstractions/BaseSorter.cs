using Application.Filters.Abstractions;
using Domain.Abstractions;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Filters.Abstractions;

internal abstract class BaseSorter<TEntity, TFilter>(CoreDbContext dbContext) : ISorter<TEntity, TFilter>
	where TFilter : IFilter
	where TEntity : class, IEntity
{
	protected readonly DbSet<TEntity> _entities = dbContext.Set<TEntity>();
	protected readonly CoreDbContext _dbContext = dbContext;

	public abstract IQueryable<TEntity> GetSort(TFilter filter);

	protected static List<string> GetLikeTermsToSearch(string rawSearchTerm) =>
		[.. rawSearchTerm.Split([' '], StringSplitOptions.RemoveEmptyEntries).Select(t => $"%{t}%")];
}