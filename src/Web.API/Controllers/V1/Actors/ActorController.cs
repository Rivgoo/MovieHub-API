using Application.Actors.Abstractions;
using Asp.Versioning;
using AutoMapper;
using Domain.Entities;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.API.Controllers.V1.Actors.Requests;
using Web.API.Controllers.V1.Actors.Responses;
using Web.API.Core.BaseResponses;
using Web.API.Core;
using Application.Results;
using Application.Actors;
using Application.Filters.Abstractions;
using Application.Actors.Dtos;
using Application.Filters;

namespace Web.API.Controllers.V1.Actors;

/// <summary>
/// API Controller for managing Actor entities.
/// </summary>
/// <summary>
/// Initializes a new instance of the <see cref="ActorController"/> class.
/// </summary>
/// <param name="mapper">The AutoMapper instance for object mapping.</param>
/// <param name="entityService">The service for managing Actor entities (<see cref="IActorService"/>).</param>
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/actors")]
public class ActorController(
	IMapper mapper, 
	IActorService entityService,
	IFilterService<Actor, ActorFilter> filterService) :
	EntityApiController<IActorService>(mapper, entityService)
{
	private readonly IFilterService<Actor, ActorFilter> _filterService = filterService;

	/// <summary>
	/// Retrieves actor items based on filter, pagination, and ordering criteria.
	/// </summary>
	/// <param name="pageSize">The number of items to return per page (must be positive).</param>
	/// <param name="orderField">The field name(s) to order by (e.g., "Id", "FirstName", "LastName").</param>
	/// <param name="orderType">The order type(s) corresponding to each orderField.</param>
	/// <param name="filter">The filter criteria object.</param>
	/// <response code="200">Returns the paginated list of actor items.</response>
	/// <response code="400">Returns an error if the input (page size, ordering, filter) is invalid.</response>
	[AllowAnonymous]
	[HttpGet("filter")]
	[ProducesResponseType(typeof(PaginatedList<ActorDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ByFilter(int pageSize,
		[FromQuery] string[] orderField, [FromQuery] List<QueryableOrderType> orderType, [FromQuery] ActorFilter filter)
	{
		if (orderField == null || orderField.Length == 0)
		{
			orderField = ["LastName", "FirstName"];
			orderType = [QueryableOrderType.OrderBy, QueryableOrderType.ThenBy];
		}

		for (var i = 0; i < orderField.Length; i++)
		{
			var field = orderField[i];

			if (orderType.Count <= i)
				return Result.Bad(FilterErrors.InvalidOrderInput).ToActionResult();

			var type = orderType[i];
			var result = filter.AddOrdering(type, field);

			if (result.IsFailure)
				return result.ToActionResult();
		}

		var filterResult = await _filterService.SetPageSize(pageSize)
			.AddFilter(filter)
			.ApplyAsync<ActorDto, IActorSelector>();

		if (filterResult.IsSuccess)
			foreach (var actor in filterResult.Value!.Items)
				actor.PhotoUrl = CreateFullPhotoUrl(actor.PhotoUrl);

		return filterResult.ToActionResult();
	}


	/// <summary>
	/// Checks if a Actor entity with the specified ID exists.
	/// </summary>
	/// <summary>
	/// This endpoint performs an existence check without retrieving the full entity.
	/// Requires authentication.
	/// </summary>
	/// <param name="id">The ID of the Actor entity to check.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult indicating whether the Actor entity exists.</returns>
	/// <response code="200">Returns a <c>ExistsResponse</c> with <see langword="true"/> if the entity exists, or <see langword="false"/> otherwise.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required authorization.</response>
	[HttpGet("{id}/exists")]
	[ProducesResponseType(typeof(ExistsResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> ExistsById(int id, CancellationToken cancellationToken)
		=> Ok(new ExistsResponse(await _entityService.ExistsByIdAsync(id, cancellationToken)));

	/// <summary>
	/// Retrieves a specific Actor entity by its unique identifier.
	/// </summary>
	/// <summary>
	/// Returns the actor with the matching ID.
	/// Requires authentication.
	/// </summary>
	/// <param name="id">The ID of the Actor entity to retrieve.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult containing the actor or an error if not found.</returns>
	/// <response code="200">Returns the <c>Actor</c> entity.</response>
	/// <response code="400">If the provided ID is invalid (e.g., format error, if using complex IDs).</response> 
	/// <response code="404">If the entity with the specified ID is not found.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required authorization.</response>
	[AllowAnonymous]
	[HttpGet("{id}")]
	[ProducesResponseType(typeof(ActorDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
	{
		var result = await _entityService.GetByIdAsync(id, cancellationToken);

		return result.Match(
			actor => {
				var response = _mapper.Map<ActorDto>(actor);
				response.PhotoUrl = CreateFullPhotoUrl(response.PhotoUrl);
				return Ok(response);
			},
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Creates a new Actor entity.
	/// </summary>
	/// <summary>
	/// Creates a new actor based on the provided data.
	/// Requires authentication.
	/// </summary>
	/// <param name="request">The request model (<c>CreateActorRequest</c>) for the new actor.</param>
	/// <returns>An IActionResult containing the ID of the created actor or an error.</returns>
	/// <response code="201">Returns a <c>CreatedResponse</c> with the ID of the newly created actor.</response>
	/// <response code="400">Returns an <c>Error</c> object for validation failures (e.g., name already exists, invalid format).</response>
	/// <response code="409">Returns an <c>Error</c> object if a conflict occurs (e.g., resource with same unique identifier already exists).</response> // Якщо IActorService повертає ErrorType.Conflict
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required authorization.</response>
	[HttpPost]
	[ProducesResponseType(typeof(CreatedResponse<int>), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
	[Authorize(Roles = RoleList.Admin)]
	public async Task<IActionResult> Create([FromBody] CreateActorRequest request)
	{
		var actorToCreate = _mapper.Map<Actor>(request);

		var result = await _entityService.CreateAsync(actorToCreate);

		return result.Match(
			_ => Ok(new CreatedResponse<int>(result.Value!.Id)),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Updates an existing Actor entity.
	/// </summary>
	/// <summary>
	/// Updates the actor with the matching ID using the provided data.
	/// Requires authentication.
	/// </summary>
	/// <param name="id">The ID of the Actor entity to update.</param>
	/// <param name="request">The request model (<c>UpdateActorRequest</c>) with updated data.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult indicating the result of the update operation.</returns>
	/// <response code="200">Indicates successful update (no body, or updated entity body if needed).</response>
	/// <response code="400">Returns an <c>Error</c> object for validation failures (e.g., invalid format) or if the provided ID in the URL doesn't match an ID in the body (if applicable).</response>
	/// <response code="404">Returns an <c>Error</c> object if the entity with the specified ID is not found.</response>
	/// <response code="409">Returns an <c>Error</c> object if a conflict occurs during update (e.g., concurrency conflict).</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required authorization.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Update(int id, [FromBody] UpdateActorRequest request, CancellationToken cancellationToken)
	{
		var existingActorResult = await _entityService.GetByIdAsync(id, cancellationToken);

		if (existingActorResult.IsFailure)
			return existingActorResult.ToActionResult();

		var actorToUpdate = _mapper.Map(request, existingActorResult.Value);

		var result = await _entityService.UpdateAsync(actorToUpdate);

		return result.Match(
			_ => Ok(),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Deletes a specific Actor entity by its unique identifier.
	/// </summary>
	/// <summary>
	/// Deletes the actor with the matching ID.
	/// Requires authentication.
	/// </summary>
	/// <param name="id">The ID of the Actor entity to delete.</param>
	/// <response code="200">Indicates successful deletion (no body).</response>
	/// <response code="404">Returns an <c>Error</c> object if the entity with the specified ID is not found.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required authorization.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> Delete(int id)
	{
		var result = await _entityService.DeleteByIdAsync(id);

		return result.Match(
			Ok,
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Uploads or updates the photo for a specific Actor entity.
	/// </summary>
	/// <param name="id">The ID of the Actor entity for which to upload the photo.</param>
	/// <param name="request">The request containing the Base64 encoded image string.</param>
	/// <response code="200">Returns the URL of the uploaded photo.</response>
	/// <response code="400">Returns an Error object for invalid Base64 string or unsupported image format.</response>
	/// <response code="404">Returns an Error object if the Actor entity is not found.</response>
	/// <response code="500">Returns an Error object if a failure occurred during file saving.</response>
	/// <response code="401">If the user is unauthorized.</response>
	/// <response code="403">If the user is not an Admin.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpPost("{id}/photo")]
	[ProducesResponseType(typeof(UploadActorPhotoResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> UploadPhoto(int id, [FromBody] UploadActorPhotoRequest request)
	{
		var result = await _entityService.SavePhotoAsync(id, request.Base64Image);

		return result.Match(
			actor => Ok(new UploadActorPhotoResponse(CreateFullPhotoUrl(actor.PhotoUrl)!)),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Deletes the photo for a specific Actor entity.
	/// </summary>
	/// <param name="id">The ID of the Actor entity for which to delete the photo.</param>
	/// <response code="200">Indicates successful deletion.</response>
	/// <response code="404">Returns an Error object if the Actor entity is not found.</response>
	/// <response code="500">Returns an Error object if a failure occurred during file deletion.</response>
	/// <response code="401">If the user is unauthorized.</response>
	/// <response code="403">If the user is not an Admin.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpDelete("{id}/photo")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> DeletePhoto(int id)
	{
		var result = await _entityService.DeletePhotoAsync(id);

		return result.Match(
			Ok,
			error => result.ToActionResult()
		);
	}

	private string? CreateFullPhotoUrl(string? relativePath)
	{
		if (string.IsNullOrWhiteSpace(relativePath))
			return null;

		if (HttpContext == null)
			return relativePath;

		var request = HttpContext.Request;
		var baseUrl = $"{request.Scheme}://{request.Host}";
		var path = relativePath.StartsWith('/') ? relativePath : $"/{relativePath}";

		return $"{baseUrl}{path}";
	}
}