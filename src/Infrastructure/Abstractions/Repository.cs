using Application.Abstractions.Repositories;
using Domain.Abstractions;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Abstractions;

/// <summary>
/// Represents a base abstract class for repositories that interact with a database context.
/// </summary>
/// <remarks>
/// This class provides access to the underlying <see cref="CoreDbContext"/>.
/// Concrete repository implementations or more specialized base classes
/// should inherit from this class.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="Repository"/> class.
/// </remarks>
/// <param name="dbContext">The database context instance.</param>
internal abstract class Repository(CoreDbContext dbContext) : IRepository
{
	/// <summary>
	/// The database context used by the repository.
	/// </summary>
	protected readonly CoreDbContext _dbContext = dbContext;
}

/// <summary>
/// Provides an abstract base class for repositories that manage entities with a specific type and identifier type.
/// </summary>
/// <typeparam name="TEntity">The type of the entity managed by this repository.</typeparam>
/// <typeparam name="TId">The type of the entity's unique identifier.</typeparam>
/// <remarks>
/// This class inherits from <see cref="Repository"/> and provides access to a typed <see cref="DbSet{TEntity}"/>
/// for the managed entity type. It enforces constraints on the entity and identifier types.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="Repository{TEntity}"/> class.
/// </remarks>
/// <param name="dbContext">The database context instance.</param>
internal abstract class Repository<TEntity>(CoreDbContext dbContext) : Repository(dbContext)
	where TEntity : class, IEntity
{
	/// <summary>
	/// The database set for the managed entity type.
	/// </summary>
	protected readonly DbSet<TEntity> _entities = dbContext.Set<TEntity>();
}