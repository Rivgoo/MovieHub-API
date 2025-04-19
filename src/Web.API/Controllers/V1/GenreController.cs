using Application.Genres.Abstractions;
using Asp.Versioning;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Web.API.Core;
using Web.API.Core.BaseResponses;

namespace Web.API.Controllers.V1;

/// <summary>
/// API Controller for managing Genre entities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GenreController"/> class.
/// </remarks>
/// <param name="mapper">The AutoMapper instance for object mapping.</param>
/// <param name="entityService">The service for managing Genre entities.</param>
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/genres")]
public class GenreController(IMapper mapper, IGenreService entityService) :
	EntityApiController<Genre, int>(mapper, entityService)
{
	/// <summary>
	/// Checks if a Genre entity with the specified ID exists.
	/// </summary>
	/// <param name="id">The ID of the Genre entity to check.</param>
	/// <returns>A response indicating whether the Genre entity exists.</returns>
	/// <response code="200">Returns <see cref="ExistsResponse"/> with <see langword="true"/> if the entity exists, or <see langword="false"/> otherwise.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	[HttpGet("{id}/exists")]
	[ProducesResponseType(typeof(ExistsResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> ExistsById(int id)
		=> Ok(new ExistsResponse(await _entityService.ExistsByIdAsync(id)));
}