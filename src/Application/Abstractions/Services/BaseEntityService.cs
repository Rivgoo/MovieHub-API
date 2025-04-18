using Application.Abstractions.Repositories;
using Application.Results;
using Domain.Abstractions;

namespace Application.Abstractions.Services;

public abstract class BaseEntityService<TEntity, TId, TRepository>(TRepository entityRepository, IUnitOfWork unitOfWork)
	: IEntityService<TEntity, TId>
	where TEntity : BaseEntity<TId>
	where TId : IComparable<TId>
	where TRepository : IEntityOperations<TEntity, TId>
{
	protected abstract Func<TId, Error> EntityWithIdNotFound { get; }
	protected abstract Error CreateNullFailure { get;  }
	protected abstract Error UpdateNullFailure { get; }

	protected TRepository _entityRepository = entityRepository;
	protected IUnitOfWork _unitOfWork = unitOfWork;

	public virtual async Task<bool> ExistsByIdAsync(TId? entityId, CancellationToken cancellationToken = default)
		=> await _entityRepository.ExistByIdAsync(entityId!, cancellationToken);
	public virtual async Task<Result> VerifyExistsByIdAsync(TId? entityId, CancellationToken cancellationToken = default)
	{
		if (await ExistsByIdAsync(entityId, cancellationToken) == true)
			return Result.Ok();

		return Result.Bad(EntityWithIdNotFound(entityId!));
	}

	public virtual async Task<Result<TEntity>> GetByIdAsync(TId entityId, CancellationToken cancellationToken = default)
	{
		var entity = await _entityRepository.GetByIdAsync(entityId, cancellationToken);

		if (entity == null)
			return Result<TEntity>.Bad(EntityWithIdNotFound(entityId));

		return Result<TEntity>.Ok(entity);
	}
	public virtual async Task<ICollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
		=> await _entityRepository.GetAllAsync(cancellationToken);

	public virtual async Task<Result<TEntity>> CreateAsync(TEntity newEntity)
	{
		if (newEntity == null)
			return Result<TEntity>.Bad(CreateNullFailure);

		var validationResult = await ValidateEntityAsync(newEntity);

		if (validationResult.IsFailure)
			return validationResult.ToValue<TEntity>();

		_entityRepository.Add(newEntity);
		await _unitOfWork.SaveChangesAsync();

		return Result<TEntity>.Ok(newEntity);
	}
	public virtual async Task<Result<TEntity>> UpdateAsync(TEntity changedEntity)
	{
		if(changedEntity == null)
			return Result<TEntity>.Bad(UpdateNullFailure);

		var entityExistsResult = await VerifyExistsByIdAsync(changedEntity.Id);

		if (entityExistsResult.IsFailure) return entityExistsResult.ToValue<TEntity>();

		var validationResult = await ValidateEntityAsync(changedEntity);

		if (validationResult.IsFailure)
			return validationResult.ToValue<TEntity>();

		_entityRepository.Update(changedEntity);
		await _unitOfWork.SaveChangesAsync();

		return Result<TEntity>.Ok(changedEntity);
	}
	public virtual async Task<Result> DeleteByIdAsync(TId entityId)
	{
		var entityResult = await GetByIdAsync(entityId);

		if (entityResult.IsFailure) return entityResult;

		return await DeleteEntityAsync(entityResult.Value!);
	}

	protected abstract Task<Result> ValidateEntityAsync(TEntity entity);
	protected virtual async Task<Result> DeleteEntityAsync(TEntity entity)
	{
		_entityRepository.Remove(entity);
		await _unitOfWork.SaveChangesAsync();

		return Result.Ok();
	}
}