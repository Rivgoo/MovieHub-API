using Application.Genres.Abstractions;
using Asp.Versioning;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.API.Core;
using Web.API.Core.BaseResponses;

namespace Web.API.Controllers.V1;

[ApiVersion("1")]
[Route("api/v{version:apiVersion}/genres")]
public class GenreController(IMapper mapper, IGenreService entityService) : 
	EntityApiController<Genre, int>(mapper, entityService)
{
	/// <summary>
	/// Checks if a entity with the specified ID exists.
	/// </summary>
	/// <param name="id">The ID of the place to check.</param>
	/// <returns>A response indicating whether the place exists.</returns>
	/// <response code="200">Returns if the entity exists.</response>
	/// <response code="401">Returns if the user is unauthorized.</response>
	[Authorize]
	[HttpGet("{id}/exists")]
	[ProducesResponseType(typeof(ExistsResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> ExistsById(int id)
		=> Ok(new ExistsResponse(await _entityService.ExistsByIdAsync(id)));
}