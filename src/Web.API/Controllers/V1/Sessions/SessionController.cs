﻿using Application.Sessions.Abstractions;
using Asp.Versioning;
using AutoMapper;
using Domain.Entities;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.API.Controllers.V1.Sessions.Requests;
using Web.API.Core.BaseResponses;
using Web.API.Core;
using Application.Results;
using Application.Filters.Abstractions;
using Application.Filters;
using Application.Sessions;
using Application.Sessions.Models;
using Application.Sessions.Dtos;
using Domain.Enums;

namespace Web.API.Controllers.V1.Sessions;

/// <summary>
/// API Controller for managing Session entities.
/// </summary>
/// <param name="mapper">The AutoMapper instance for object mapping.</param>
/// <param name="entityService">The service for managing Session entities (<see cref="ISessionService"/>).</param>
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/sessions")]
public class SessionController(
	IMapper mapper, 
	ISessionService entityService,
	IFilterService<Session, SessionFilter> filterService,
	IFilterService<Session, SessionContentFilter> sessionContentfilterService) :
	EntityApiController<ISessionService>(mapper, entityService)
{
	private readonly IFilterService<Session, SessionFilter> _filterService = filterService;
	private readonly IFilterService<Session, SessionContentFilter> _sessionContentfilterService = sessionContentfilterService;

	/// <summary>
	/// Retrieves session items based on filter, pagination, and ordering criteria.
	/// </summary>
	/// <param name="pageSize">The number of items to return per page (must be positive).</param>
	/// <param name="orderField">The field name(s) to order by (e.g., "Id", "StartTime", "TicketPrice").</param>
	/// <param name="orderType">The order type(s) corresponding to each orderField.</param>
	/// <param name="filter">The filter criteria object.</param>
	/// <response code="200">Returns the paginated list of session items.</response>
	/// <response code="400">Returns an error if the input (page size, ordering, filter) is invalid.</response>
	[AllowAnonymous]
	[HttpGet("filter")]
	[ProducesResponseType(typeof(PaginatedList<SessionDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ByFilter(int pageSize,
		[FromQuery] string[] orderField, [FromQuery] List<QueryableOrderType> orderType, [FromQuery] SessionFilter filter)
	{
		filter.Status ??= SessionStatus.Scheduled;

		if (orderField == null || orderField.Length == 0)
		{
			orderField = ["StartTime"];
			orderType = [QueryableOrderType.OrderBy];
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

		var filterResult = await _filterService
			.SetPageSize(pageSize)
			.AddFilter(filter)
			.ApplyAsync<SessionDto, ISessionSelector>();

		return filterResult.ToActionResult();
	}

	/// <summary>
	/// Retrieves session items along with their associated content details, based on filtering, pagination, and ordering criteria.
	/// </summary>
	/// <remarks>
	/// This endpoint allows anonymous access and returns a paginated list combining session information with details of the content being shown.
	/// Default sort order is by StartTime ascending if no specific order is provided.
	/// </remarks>
	/// <param name="pageSize">The number of items to return per page (must be positive).</param>
	/// <param name="orderField">The field name(s) to order by (e.g., "StartTime", "TicketPrice", "Content.Title", "Content.ReleaseYear"). Provided via query string.</param>
	/// <param name="orderType">The order type(s) corresponding to each orderField (e.g., OrderBy, ThenByDescending). Provided via query string.</param>
	/// <param name="filter">The filter criteria object (SessionContentFilter) containing properties to filter sessions and their content. Provided via query string.</param>
	/// <response code="200">Returns the paginated list of session items with associated content details.</response>
	/// <response code="400">Returns an error if the input for pagination, ordering, or filtering is invalid.</response>
	[AllowAnonymous]
	[HttpGet("filter-with-content")]
	[ProducesResponseType(typeof(PaginatedList<SessionContentDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ByFilterWithContent(int pageSize,
	[FromQuery] string[] orderField, [FromQuery] List<QueryableOrderType> orderType, [FromQuery] SessionContentFilter filter)
	{
		filter.Status ??= SessionStatus.Scheduled;

		if (orderField == null || orderField.Length == 0)
		{
			orderField = ["StartTime"];
			orderType = [QueryableOrderType.OrderBy];
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

		var filterResult = await _sessionContentfilterService
			.SetPageSize(pageSize)
			.AddFilter(filter)
			.ApplyAsync<SessionContentDto, ISessionContentSelector>();

		foreach (var item in filterResult.Value!.Items)
		{
			item.PosterUrl = CreateFullImageUrl(item.PosterUrl);
			item.BannerUrl = CreateFullImageUrl(item.BannerUrl);
		}

		return filterResult.ToActionResult();
	}

	/// <summary>
	/// Checks if a Session entity with the specified ID exists.
	/// </summary>
	/// <summary>
	/// This endpoint performs an existence check without retrieving the full entity.
	/// Requires authentication.
	/// </summary>
	/// <param name="id">The ID of the Session entity to check.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult indicating whether the Session entity exists.</returns>
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
	/// Retrieves a list of all Session entities.
	/// </summary>
	/// <summary>
	/// Returns a collection of all available sessions.
	/// Requires authentication.
	/// </summary>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult containing the list of sessions.</returns>
	/// <response code="200">Returns a list of <c>Session</c> entities.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required authorization.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<SessionDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
	{
		return Ok(await _entityService.GetAllAsync(cancellationToken));
	}

	/// <summary>
	/// Retrieves a specific Session entity by its unique identifier.
	/// </summary>
	/// <summary>
	/// Returns the session with the matching ID.
	/// Requires authentication.
	/// </summary>
	/// <param name="id">The ID of the Session entity to retrieve.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>An IActionResult containing the session or an error if not found.</returns>
	/// <response code="200">Returns the <c>Session</c> entity.</response>
	/// <response code="400">If the provided ID is invalid (e.g., format error, if using complex IDs).</response> 
	/// <response code="404">If the entity with the specified ID is not found.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the required authorization.</response>
	[AllowAnonymous]
	[HttpGet("{id}")]
	[ProducesResponseType(typeof(SessionDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
	{
		var result = await _entityService.GetByIdAsync(id, cancellationToken);

		return result.Match(
			session => Ok(_mapper.Map<SessionDto>(session)),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Creates a new Session entity.
	/// </summary>
	/// <summary>
	/// Creates a new session based on the provided data.
	/// Requires authentication.
	/// </summary>
	/// <param name="request">The request model (<c>CreateSessionRequest</c>) for the new session.</param>
	/// <returns>An IActionResult containing the ID of the created session or an error.</returns>
	/// <response code="201">Returns a <c>CreatedResponse</c> with the ID of the newly created session.</response>
	/// <response code="400">Returns an <c>Error</c> object for validation failures (e.g., name already exists, invalid format).</response>
	/// <response code="409">Returns an <c>Error</c> object if a conflict occurs (e.g., resource with same unique identifier already exists).</response> // Якщо ISessionService повертає ErrorType.Conflict
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
	public async Task<IActionResult> Create([FromBody] CreateSessionRequest request)
	{
		var sessionToCreate = _mapper.Map<Session>(request);

		var result = await _entityService.CreateAsync(sessionToCreate);

		return result.Match(
			_ => Ok(new CreatedResponse<int>(result.Value.Id)),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Updates an existing Session entity.
	/// </summary>
	/// <summary>
	/// Updates the session with the matching ID using the provided data.
	/// Requires authentication.
	/// </summary>
	/// <param name="id">The ID of the Session entity to update.</param>
	/// <param name="request">The request model (<c>UpdateSessionRequest</c>) with updated data.</param>
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
	public async Task<IActionResult> Update(int id, [FromBody] UpdateSessionRequest request, CancellationToken cancellationToken)
	{
		var existedSessionResult = await _entityService.GetByIdAsync(id, cancellationToken);

		if (existedSessionResult.IsFailure)
			return existedSessionResult.ToActionResult();

		var updatedSession = _mapper.Map(request, existedSessionResult.Value);

		var result = await _entityService.UpdateAsync(updatedSession);

		return result.Match(
			_ => Ok(),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Deletes a specific Session entity by its unique identifier.
	/// </summary>
	/// <summary>
	/// Deletes the session with the matching ID.
	/// Requires authentication.
	/// </summary>
	/// <param name="id">The ID of the Session entity to delete.</param>
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

	private string? CreateFullImageUrl(string? relativePath)
	{
		if (string.IsNullOrWhiteSpace(relativePath))
			return null;

		var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

		relativePath = relativePath.StartsWith('/') ? relativePath : $"/{relativePath}";

		return $"{baseUrl}{relativePath}";
	}
}