using Asp.Versioning;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc; 
using Web.API.Core; 
using Web.API.Core.BaseResponses;
using Application.Results;
using Microsoft.AspNetCore.Authorization;
using Domain;
using Web.API.Controllers.V1.Contents.Requests;
using Application.Contents.Abstractions.Services;
using Web.API.Controllers.V1.Contents.Responses;

namespace Web.API.Controllers.V1.Contents;

/// <summary>
/// API Controller for managing Content entities.
/// </summary>
/// <summary>
/// Initializes a new instance of the <see cref="ContentController"/> class.
/// </summary>
/// <param name="mapper">The AutoMapper instance for object mapping.</param>
/// <param name="entityService">The service for managing Content entities (<see cref="IContentService"/>).</param>
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/contents")]
public class ContentController(IMapper mapper, IContentService entityService) :
	EntityApiController<Content, int>(mapper, entityService)
{
	/// <summary>
	/// Checks if a Content entity with the specified ID exists.
	/// </summary>
	/// <summary>
	/// This endpoint performs an existence check without retrieving the full entity.
	/// Requires authentication.
	/// </summary>
	/// <param name="id">The ID of the Content entity to check.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult indicating whether the Content entity exists.</returns>
	/// <response code="200">Returns a <c>ExistsResponse</c> with <see langword="true"/> if the entity exists, or <see langword="false"/> otherwise.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required authorization.</response>
	[AllowAnonymous]
	[HttpGet("{id}/exists")]
	[ProducesResponseType(typeof(ExistsResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> ExistsById(int id, CancellationToken cancellationToken)
		=> Ok(new ExistsResponse(await _entityService.ExistsByIdAsync(id, cancellationToken)));

	/// <summary>
	/// Retrieves a list of all Content entities.
	/// </summary>
	/// <summary>
	/// Returns a collection of all available content items.
	/// Requires authentication.
	/// </summary>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult containing the list of content items.</returns>
	/// <response code="200">Returns a list of <c>ContentResponse</c> DTOs.</response> // Повертаємо DTO колекцію
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required authorization.</response>
	[AllowAnonymous]
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<ContentResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
	{
		var contentItems = await _entityService.GetAllAsync(cancellationToken);

		return Ok(_mapper.Map<IEnumerable<ContentResponse>>(contentItems));
	}

	/// <summary>
	/// Retrieves a specific Content entity by its unique identifier.
	/// </summary>
	/// <summary>
	/// Returns the content item with the matching ID.
	/// Requires authentication.
	/// </summary>
	/// <param name="id">The ID of the Content entity to retrieve.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult containing the content item DTO or an error if not found.</returns>
	/// <response code="200">Returns the <c>ContentResponse</c> DTO.</response> // Повертаємо DTO
	/// <response code="400">If the provided ID is invalid (e.g., format error, if using complex IDs).</response> // Optional, depends on binding
	/// <response code="404">If the entity with the specified ID is not found.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required authorization.</response>
	[AllowAnonymous]
	[HttpGet("{id}")]
	[ProducesResponseType(typeof(ContentResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
	{
		var result = await _entityService.GetByIdAsync(id, cancellationToken);

		return result.Match(
			content => Ok(_mapper.Map<ContentResponse>(content)),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Creates new Content entity.
	/// </summary>
	/// <summary>
	/// Creates a new content item based on the provided data.
	/// Requires authentication with the Admin role.
	/// </summary>
	/// <param name="request">The request model (<c>CreateContentRequest</c>) for the new content item.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult containing the ID of the created content item or an error.</returns>
	/// <response code="201">Returns a <c>CreatedResponse</c> with the ID of the newly created content item.</response> // 201 Created для успішного створення
	/// <response code="400">Returns an <c>Error</c> object for validation failures (e.g., required fields missing, invalid format).</response>
	/// <response code="409">Returns an <c>Error</c> object if a conflict occurs (e.g., content item with same unique identifier already exists, if applicable).</response> // Якщо IContentService повертає ErrorType.Conflict
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required 'Admin' role.</response>
	[HttpPost]
	[ProducesResponseType(typeof(CreatedResponse<int>), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
	[Authorize(Roles = RoleList.Admin)]
	public async Task<IActionResult> Create([FromBody] CreateContentRequest request, CancellationToken cancellationToken)
	{
		var contentToCreate = _mapper.Map<Content>(request);

		var result = await _entityService.CreateAsync(contentToCreate);

		return result.Match(
			_ => Ok(new CreatedResponse<int>(result.Value.Id)),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Updates an existing Content entity.
	/// </summary>
	/// <summary>
	/// Updates the content item with the matching ID using the provided data.
	/// Requires authentication with the Admin role.
	/// </summary>
	/// <param name="id">The ID of the Content entity to update.</param>
	/// <param name="request">The request model (<c>UpdateContentRequest</c>) with updated data.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult indicating the result of the update operation.</returns>
	/// <response code="200">Indicates successful update (no body).</response> // 200 OK для успішного оновлення
	/// <response code="400">Returns an <c>Error</c> object for validation failures (e.g., invalid format) or if the provided ID in the URL doesn't match an ID in the body (if applicable).</response>
	/// <response code="404">Returns an <c>Error</c> object if the entity with the specified ID is not found.</response>
	/// <response code="409">Returns an <c>Error</c> object if a conflict occurs during update (e.g., concurrency conflict).</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required 'Admin' role.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Update(int id, [FromBody] UpdateContentRequest request, CancellationToken cancellationToken)
	{
		var existingContentResult = await _entityService.GetByIdAsync(id, cancellationToken);

		if (existingContentResult.IsFailure) 
			return existingContentResult.ToActionResult();

		var contentToUpdate = _mapper.Map(request, existingContentResult.Value);

		var result = await _entityService.UpdateAsync(contentToUpdate);

		return result.Match(
			_ => Ok(),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Deletes a specific Content entity by its unique identifier.
	/// </summary>
	/// <summary>
	/// Deletes the content item with the matching ID.
	/// Requires authentication with the Admin role.
	/// </summary>
	/// <param name="id">The ID of the Content entity to delete.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult indicating the result of the deletion operation.</returns>
	/// <response code="200">Indicates successful deletion (no body).</response> // 200 OK для успішного видалення
	/// <response code="404">Returns an <c>Error</c> object if the entity with the specified ID is not found.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required 'Admin' role.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
	{
		var result = await _entityService.DeleteByIdAsync(id);

		return result.Match(
			Ok,
			error => result.ToActionResult()
		);
	}
}