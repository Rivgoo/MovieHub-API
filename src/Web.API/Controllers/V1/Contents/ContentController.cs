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
using Application.Contents.Dtos;

namespace Web.API.Controllers.V1.Contents;

/// <summary>
/// API Controller for managing Content entities.
/// </summary>
/// <param name="mapper">The AutoMapper instance for object mapping.</param>
/// <param name="entityService">The service for managing Content entities (<see cref="IContentService"/>).</param>
/// <param name="configuration">Configuration settings for the application.</param>
/// <param name="contentActorService"> The service for managing ContentActor entities (<see cref="IContentActorService"/>).</param>
/// <param name="contentGenreService"> The service for managing ContentGenre entities (<see cref="IContentGenreService"/>).</param>
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/contents")]
public class ContentController(
	IContentGenreService contentGenreService,
	IContentActorService contentActorService,
	IConfiguration configuration,
	IMapper mapper,
	IContentService entityService) :
	EntityApiController<IContentService>(mapper, entityService)
{
	private readonly IConfiguration _configuration = configuration;
	private readonly IContentGenreService _contentGenreService = contentGenreService;
	private readonly IContentActorService _contentActorService = contentActorService;

	/// <summary>
	/// Seeds fake Content entities into the database.
	/// </summary>
	/// <response code="200">Indicates successful seeding (no body).</response>
	/// <response code="500">Returns an <c>Error</c> object if a failure occurred during creation or poster saving.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required 'Admin' role.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpPost("seed-fake-data")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> SeedFakeData(int number)
	{
		var isEnabled = _configuration.GetValue<bool>("FakeDataSeed");

		if (!isEnabled)
			return BadRequest("Fake data seeding is disabled.");

		number = Math.Clamp(number, 1, 100);
		var random = new Random();
		var posterBase64 = "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";

		foreach (var i in Enumerable.Range(1, number))
		{
			var content = new Content
			{
				Title = $"Content {i}",
				Description = $"Description for Content {i}",
				Rating = random.Next(0, 100),
				ReleaseYear = 2000 + random.Next(0, 30),
				TrailerUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
				DurationMinutes = random.Next(3, 250)
			};

			var result = await _entityService.CreateAsync(content);

			if (result.IsFailure)
				return result.ToActionResult();

			await _entityService.SavePosterAsync(result.Value.Id, posterBase64);
		}

		return Ok();
	}

	/// <summary>
	/// Checks if a Content entity with the specified ID exists.
	/// </summary>
	/// <param name="id">The ID of the Content entity to check.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <response code="200">Returns a <c>ExistsResponse</c> with <see langword="true"/> if the entity exists, or <see langword="false"/> otherwise.</response>
	[AllowAnonymous]
	[HttpGet("{id}/exists")]
	[ProducesResponseType(typeof(ExistsResponse), StatusCodes.Status200OK)]
	public async Task<IActionResult> ExistsById(int id, CancellationToken cancellationToken)
		=> Ok(new ExistsResponse(await _entityService.ExistsByIdAsync(id, cancellationToken)));

	/// <summary>
	/// Retrieves a list of all Content entities.
	/// </summary>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <response code="200">Returns a list of <c>ContentDto</c> DTOs.</response> // Повертаємо DTO колекцію
	[AllowAnonymous]
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<ContentDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
	{
		var contentItems = await _entityService.GetAllContentDtosAsync(cancellationToken);

		foreach (var item in contentItems)
			item.PosterUrl = CreateFullPosterUrl(item.PosterUrl);

		return Ok(contentItems);
	}

	/// <summary>
	/// Retrieves a specific Content entity by its unique identifier.
	/// </summary>
	/// <param name="id">The ID of the Content entity to retrieve.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <response code="200">Returns the <c>ContentDto</c> DTO.</response> // Повертаємо DTO
	/// <response code="404">If the entity with the specified ID is not found.</response>
	[AllowAnonymous]
	[HttpGet("{id}")]
	[ProducesResponseType(typeof(ContentDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
	{
		var result = await _entityService.GetContentDtoAsync(id, cancellationToken);

		var content = result.Value;

		content!.PosterUrl = CreateFullPosterUrl(content.PosterUrl);

		return result.Match(
			_ => Ok(content),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Creates new Content entity.
	/// </summary>
	/// <param name="request">The request model (<c>CreateContentRequest</c>) for the new content item.</param>
	/// <response code="201">Returns a <c>CreatedResponse</c> with the ID of the newly created content item.</response>
	/// <response code="400">Returns an <c>Error</c> object for validation failures (e.g., required fields missing, invalid format).</response>
	/// <response code="409">Returns an <c>Error</c> object if a conflict occurs (e.g., content item with same unique identifier already exists, if applicable).</response>
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
	public async Task<IActionResult> Create([FromBody] CreateContentRequest request)
	{
		var contentToCreate = _mapper.Map<Content>(request);

		var result = await _entityService.CreateAsync(contentToCreate);

		if (result.IsSuccess)
		{
			foreach (var genreId in request.GenreIds)
			{
				var contentGenre = new ContentGenre
				{
					ContentId = result.Value.Id,
					GenreId = genreId
				};

				var genreResult = await _contentGenreService.CreateAsync(contentGenre);

				if (genreResult.IsFailure)
					return genreResult.ToActionResult();
			}

			foreach (var actorId in request.ActorIds)
			{
				var contentActor = new ContentActor
				{
					ContentId = result.Value.Id,
					ActorId = actorId
				};

				var actorResult = await _contentActorService.CreateAsync(contentActor);

				if (actorResult.IsFailure)
					return actorResult.ToActionResult();
			}
		}

		return result.Match(
			_ => Ok(new CreatedResponse<int>(result.Value.Id)),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Updates an existing Content entity.
	/// </summary>
	/// <param name="id">The ID of the Content entity to update.</param>
	/// <param name="request">The request model (<c>UpdateContentRequest</c>) with updated data.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
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
	/// <param name="id">The ID of the Content entity to delete.</param>
	/// <response code="200">Indicates successful deletion (no body).</response>
	/// <response code="404">Returns an <c>Error</c> object if the entity with the specified ID is not found.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required 'Admin' role.</response>
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
	/// Uploads or updates the poster image for a specific Content entity.
	/// </summary>
	/// <param name="id">The ID of the Content entity for which to upload the poster.</param>
	/// <param name="base64String">The poster image content encoded as a Base64 string in the request body.</param>
	/// <response code="200">Indicates successful upload and update (no body, or updated content entity body if needed).</response>
	/// <response code="400">Returns an <c>Error</c> object for invalid Base64 string or unsupported image format (<c>FileErrors</c>).</response>
	/// <response code="404">Returns an <c>Error</c> object if the Content entity with the specified ID is not found.</response>
	/// <response code="500">Returns an <c>Error</c> object if a failure occurred during file saving or updating the entity (<c>FileErrors</c>).</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required 'Admin' role.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpPost("{id}/poster")]
	[ProducesResponseType(typeof(UploadPosterResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> UploadPoster(int id, [FromBody] string base64String)
	{
		var result = await _entityService.SavePosterAsync(id, base64String);

		return result.Match(
			_ => Ok(new UploadPosterResponse(CreateFullPosterUrl(result.Value.PosterUrl))),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Deletes the poster image for a specific Content entity.
	/// </summary>
	/// <summary>
	/// This endpoint deletes the image file from the server's static files directory
	/// and clears the PosterUrl on the Content entity.
	/// Requires authentication with the Admin role.
	/// </summary>
	/// <param name="id">The ID of the Content entity for which to delete the poster.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult indicating the result of the deletion operation.</returns>
	/// <response code="200">Indicates successful deletion (no body).</response>
	/// <response code="404">Returns an <c>Error</c> object if the Content entity is not found (optional, if DeletePosterAsync returns NotFound).</response>
	/// <response code="500">Returns an <c>Error</c> object if a failure occurred during file deletion or updating the entity (<c>FileErrors</c>).</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required 'Admin' role.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpDelete("{id}/poster")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> DeletePoster(int id, CancellationToken cancellationToken)
	{
		var result = await _entityService.DeletePosterAsync(id);

		return result.Match(
			Ok,
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Links a specific Genre to a Content entity.
	/// </summary>
	/// <param name="id">The ID of the Content entity.</param>
	/// <param name="genreId">The ID of the Genre to link.</param>
	/// <response code="200">Indicates successful linking (no body).</response>
	/// <response code="400">Returns an Error object if input IDs are invalid or linking fails due to business rules (e.g., genre does not exist).</response>
	/// <response code="404">Returns an Error object if the Content entity with the specified ID is not found.</response>
	/// <response code="409">Returns an Error object if the relationship already exists.</response> 
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required 'Admin' role.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpPost("{id}/genres/{genreId}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> AddGenre(int id, int genreId)
	{
		var contentGenre = new ContentGenre
		{
			ContentId = id,
			GenreId = genreId
		};

		var result = await _contentGenreService.CreateAsync(contentGenre);

		return result.Match(
			_ => Ok(),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Unlinks a specific Genre from a Content entity.
	/// </summary>
	/// <param name="id">The ID of the Content entity.</param>
	/// <param name="genreId">The ID of the Genre to unlink.</param>
	/// <response code="200">Indicates successful unlinking (no body).</response>
	/// <response code="400">Returns an Error object if input IDs are invalid.</response>
	/// <response code="404">Returns an Error object if the relationship does not exist.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required 'Admin' role.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpDelete("{id}/genres/{genreId}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> DeleteGenre(int id, int genreId)
	{
		var contentGenre = await _contentGenreService.GetByDataAsync(id, genreId);

		if (contentGenre.IsFailure)
			return contentGenre.ToActionResult();

		var result = await _contentGenreService.DeleteAsync(contentGenre.Value);

		return result.ToActionResult();
	}

	/// <summary>
	/// Links a specific Actor to a Content entity.
	/// </summary>
	/// <param name="id">The ID of the Content entity.</param>
	/// <param name="actorId">The ID of the Actor to link.</param>
	/// <returns>An IActionResult indicating the result of the operation.</returns>
	/// <response code="200">Indicates successful linking (no body).</response> // Або 201 Created
	/// <response code="400">Returns an Error object if input IDs are invalid or linking fails due to business rules.</response>
	/// <response code="404">Returns an Error object if the Content entity or Actor entity is not found.</response>
	/// <response code="409">Returns an Error object if the relationship already exists.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required 'Admin' role.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpPost("{id}/actors/{actorId}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> AddActor(int id, int actorId)
	{
		var contentActor = new ContentActor
		{
			ContentId = id,
			ActorId = actorId
		};

		var result = await _contentActorService.CreateAsync(contentActor);

		return result.Match(
			_ => Ok(),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Unlinks a specific Actor from a Content entity.
	/// </summary>
	/// <param name="id">The ID of the Content entity.</param>
	/// <param name="actorId">The ID of the Actor to unlink.</param>
	/// <returns>An IActionResult indicating the result of the operation.</returns>
	/// <response code="200">Indicates successful unlinking (no body).</response>
	/// <response code="400">Returns an Error object if input IDs are invalid.</response>
	/// <response code="404">Returns an Error object if the relationship does not exist.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required 'Admin' role.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpDelete("{id}/actors/{actorId}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> DeleteActor(int id, int actorId) 
	{
		var contentActor = await _contentActorService.GetByDataAsync(id, actorId);

		if (contentActor.IsFailure)
			return contentActor.ToActionResult();

		var result = await _contentActorService.DeleteAsync(contentActor.Value);

		return result.ToActionResult();
	}

	private string? CreateFullPosterUrl(string? relativePath)
	{
		if (string.IsNullOrWhiteSpace(relativePath))
			return null;

		var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

		relativePath = relativePath.StartsWith('/') ? relativePath : $"/{relativePath}";

		return $"{baseUrl}{relativePath}";
	}
}