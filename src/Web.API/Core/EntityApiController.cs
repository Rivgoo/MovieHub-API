using AutoMapper;
using Application.Abstractions.Services;
using Domain.Abstractions;

namespace Web.API.Core;

/// <summary>
/// Provides a common abstract base class for API controllers that manage a specific domain entity.
/// </summary>
/// <typeparam name="TEntity">The type of the entity managed by the service. Must implement <see cref="IBaseEntity{TId}"/>.</typeparam>
/// <typeparam name="TId">The type of the entity's unique identifier. Must implement <see cref="IComparable{TId}"/>.</typeparam>
/// <param name="mapper">The AutoMapper instance for object mapping.</param>
/// <param name="entityService">The service for performing operations on the managed entity type.</param>
public abstract class EntityApiController<TEntity, TId>(
	IMapper mapper,
	IEntityService<TEntity, TId> entityService) : ApiController
	where TEntity : IBaseEntity<TId> 
	where TId : IComparable<TId> 
{
	protected readonly IMapper _mapper = mapper;
	protected readonly IEntityService<TEntity, TId> _entityService = entityService;
}