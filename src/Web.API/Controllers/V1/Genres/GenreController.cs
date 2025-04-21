using Application.Genres.Abstractions;
using Asp.Versioning;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Web.API.Core;
using Web.API.Core.BaseResponses;
using Application.Results;
using Microsoft.AspNetCore.Authorization;
using Domain;
using Web.API.Controllers.V1.Genres.Requests;
using Web.API.Controllers.V1.Genres.Responses;

namespace Web.API.Controllers.V1.Genres;

/// <summary>
/// API Controller for managing Genre entities.
/// </summary>
/// <summary>
/// This controller provides endpoints for standard CRUD operations on Genre entities,
/// as well as a specific endpoint to check for entity existence by ID.
/// It uses AutoMapper for request/entity mapping and relies on IGenreService for data operations.
/// </summary>
/// <summary>
/// Initializes a new instance of the <see cref="GenreController"/> class.
/// </summary>
/// <param name="mapper">The AutoMapper instance for object mapping.</param>
/// <param name="entityService">The service for managing Genre entities (<see cref="IGenreService"/>).</param>
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/genres")]
public class GenreController(IMapper mapper, IGenreService entityService) :
	EntityApiController<IGenreService>(mapper, entityService)
{
	/// <summary>
	/// Checks if a Genre entity with the specified ID exists.
	/// </summary>
	/// <summary>
	/// This endpoint performs an existence check without retrieving the full entity.
	/// Requires authentication.
	/// </summary>
	/// <param name="id">The ID of the Genre entity to check.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult indicating whether the Genre entity exists.</returns>
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
	/// Retrieves a list of all Genre entities.
	/// </summary>
	/// <summary>
	/// Returns a collection of all available genres.
	/// Requires authentication.
	/// </summary>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult containing the list of genres.</returns>
	/// <response code="200">Returns a list of <c>Genre</c> entities.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required authorization.</response>
	[AllowAnonymous]
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<GenreResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
	{
		var genres = await _entityService.GetAllAsync(cancellationToken);

		return Ok(_mapper.Map<IEnumerable<GenreResponse>>(genres));
	}

	/// <summary>
	/// Retrieves a specific Genre entity by its unique identifier.
	/// </summary>
	/// <summary>
	/// Returns the genre with the matching ID.
	/// Requires authentication.
	/// </summary>
	/// <param name="id">The ID of the Genre entity to retrieve.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult containing the genre or an error if not found.</returns>
	/// <response code="200">Returns the <c>Genre</c> entity.</response>
	/// <response code="400">If the provided ID is invalid (e.g., format error, if using complex IDs).</response> // Optional, depends on binding
	/// <response code="404">If the entity with the specified ID is not found.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required authorization.</response>
	[AllowAnonymous]
	[HttpGet("{id}")]
	[ProducesResponseType(typeof(GenreResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
	{
		var result = await _entityService.GetByIdAsync(id, cancellationToken);

		 return result.Match(
			 genre => Ok(_mapper.Map<GenreResponse>(genre)),
			 error => result.ToActionResult()
		 );
	}

	/// <summary>
	/// Creates a new Genre entity.
	/// </summary>
	/// <summary>
	/// Creates a new genre based on the provided data.
	/// Requires authentication.
	/// </summary>
	/// <param name="request">The request model (<c>CreateGenreRequest</c>) for the new genre.</param>
	/// <returns>An IActionResult containing the ID of the created genre or an error.</returns>
	/// <response code="201">Returns a <c>CreatedResponse</c> with the ID of the newly created genre.</response> // 201 Created для успішного створення
	/// <response code="400">Returns an <c>Error</c> object for validation failures (e.g., name already exists, invalid format).</response>
	/// <response code="409">Returns an <c>Error</c> object if a conflict occurs (e.g., resource with same unique identifier already exists).</response> // Якщо IGenreService повертає ErrorType.Conflict
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
	public async Task<IActionResult> Create([FromBody] CreateGenreRequest request)
	{
		var genreToCreate = _mapper.Map<Genre>(request);

		var result = await _entityService.CreateAsync(genreToCreate);

		return result.Match(
			_ => Ok(new CreatedResponse<int>(result.Value.Id)),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Updates an existing Genre entity.
	/// </summary>
	/// <summary>
	/// Updates the genre with the matching ID using the provided data.
	/// Requires authentication.
	/// </summary>
	/// <param name="id">The ID of the Genre entity to update.</param>
	/// <param name="request">The request model (<c>UpdateGenreRequest</c>) with updated data.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult indicating the result of the update operation.</returns>
	/// <response code="200">Indicates successful update (no body, or updated entity body if needed).</response> // 200 OK для успішного оновлення
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
	public async Task<IActionResult> Update(int id, [FromBody] UpdateGenreRequest request, CancellationToken cancellationToken)
	{
		var genreToUpdate = _mapper.Map<Genre>(request);
		genreToUpdate.Id = id;

		var result = await _entityService.UpdateAsync(genreToUpdate);

		return result.Match(
			_ => Ok(),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Deletes a specific Genre entity by its unique identifier.
	/// </summary>
	/// <summary>
	/// Deletes the genre with the matching ID.
	/// Requires authentication.
	/// </summary>
	/// <param name="id">The ID of the Genre entity to delete.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult indicating the result of the deletion operation.</returns>
	/// <response code="200">Indicates successful deletion (no body).</response> // 200 OK для успішного видалення
	/// <response code="404">Returns an <c>Error</c> object if the entity with the specified ID is not found.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required authorization.</response>
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