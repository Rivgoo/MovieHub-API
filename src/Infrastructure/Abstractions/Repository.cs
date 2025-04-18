using Application.Abstractions.Repositories;
using Domain.Abstractions;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Abstractions;

internal abstract class Repository(CoreDbContext dbContext) : IRepository
{
	protected readonly CoreDbContext _dbContext = dbContext;
}

internal abstract class Repository<TEntity, TId>(CoreDbContext dbContext) : Repository(dbContext)
	where TEntity : BaseEntity<TId>
	where TId : IComparable<TId>
{
	protected readonly DbSet<TEntity> _entities = dbContext.Set<TEntity>();
}

internal abstract class Repository<TEntity>(CoreDbContext dbContext) :
	Repository<TEntity, int>(dbContext) where TEntity : BaseEntity<int>
{
}