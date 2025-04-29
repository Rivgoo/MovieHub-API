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
using Application.Filters.Abstractions;
using Application.Filters;
using Application.Contents;

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
	IMapper mapper,
	IContentService entityService,
	IFilterService<Content, ContentFilter> filterService) :
	EntityApiController<IContentService>(mapper, entityService)
{
	private readonly IContentGenreService _contentGenreService = contentGenreService;
	private readonly IContentActorService _contentActorService = contentActorService;
	private readonly IFilterService<Content, ContentFilter> _filterService = filterService;

	/// <summary>
	/// Retrieves content items based on filter, pagination, and ordering criteria.
	/// </summary>
	/// <param name="pageSize">The number of items to return per page (must be positive).</param>
	/// <param name="orderField">The field name(s) to order by (e.g., "Id", "Title", "ReleaseYear"). Case-sensitive based on implementation.</param>
	/// <param name="orderType">The order type(s) corresponding to each orderField.</param>
	/// <param name="filter">The filter criteria object containing search terms, ranges, etc.</param>
	/// <response code="200">Returns the paginated list of content items.</response>
	/// <response code="400">Returns an error if the input (page size, ordering, filter) is invalid.</response>
	/// <response code="401">Returns if the user is unauthorized.</response>
	[AllowAnonymous]
	[HttpGet("filter")]
	[ProducesResponseType(typeof(PaginatedList<ContentDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> ByFilter(int pageSize,
		[FromQuery] string[] orderField, [FromQuery] List<QueryableOrderType> orderType, [FromQuery] ContentFilter filter)
	{
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

		var contents = await _filterService
			.SplitQuery()
			.SetPageSize(pageSize)
			.AddFilter(filter)
			.ApplyAsync(x => new ContentDto
			{
				Id = x.Id,
				Title = x.Title,
				Rating = x.Rating,
				ReleaseYear = x.ReleaseYear,
				Description = x.Description,
				PosterUrl = x.PosterUrl,
				TrailerUrl = x.TrailerUrl,
				DurationMinutes = x.DurationMinutes,
				CreatedAt = x.CreatedAt,
				UpdatedAt = x.UpdatedAt,
				GenreIds = x.ContentGenres.Select(g => g.Id).ToList(),
				ActorIds = x.ContentActors.Select(a => a.Id).ToList()
			});

		if (contents.IsFailure)
			return contents.ToActionResult();

		foreach (var item in contents.Value.Items)
			item.PosterUrl = CreateFullPosterUrl(item.PosterUrl);

		return contents.ToActionResult();
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
	[Authorize(Roles = RoleList.Admin)]
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
	[HttpPost("{id}/actors")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> AddActor(int id, [FromBody] AddActorRequest request)
	{
		var contentActor = new ContentActor
		{
			ContentId = id,
			ActorId = request.ActorId,
			RoleName = request.RoleName
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