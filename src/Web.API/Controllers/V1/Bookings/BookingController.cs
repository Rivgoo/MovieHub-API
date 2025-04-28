using Application.Bookings;
using Application.Bookings.Abstractions;
using Application.Results;
using Asp.Versioning;
using AutoMapper;
using Domain;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.API.Controllers.V1.Bookings.Requests;
using Web.API.Controllers.V1.Bookings.Responses;
using Web.API.Core;
using Web.API.Core.BaseResponses;

namespace Web.API.Controllers.V1.Bookings;

/// <summary>
/// API Controller for managing Booking entities.
/// </summary>
/// <param name="mapper">The AutoMapper instance for object mapping.</param>
/// <param name="entityService">The service for managing Booking entities.</param>
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/bookings")]
public class BookingController(IMapper mapper, IBookingService entityService) :
	EntityApiController<IBookingService>(mapper, entityService)
{
	/// <summary>
	/// Checks if a Booking entity with the specified ID exists.
	/// </summary>
	/// <param name="id">The ID of the Booking entity to check.</param>
	/// <param name="cancellationToken">A CancellationToken.</param>
	/// <response code="200">Returns if the entity exists.</response>
	/// <response code="401">If the user is unauthorized.</response>
	[HttpGet("{id}/exists")]
	[ProducesResponseType(typeof(ExistsResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> ExistsById(int id, CancellationToken cancellationToken)
		=> Ok(new ExistsResponse(await _entityService.ExistsByIdAsync(id, cancellationToken)));

	/// <summary>
	/// Retrieves a list of all Booking entities. (Admin only)
	/// </summary>
	/// <param name="cancellationToken">A CancellationToken.</param>
	/// <response code="200">Returns a list of Booking entities.</response>
	/// <response code="401">If the user is unauthorized.</response>
	/// <response code="403">If the user is not an Admin.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpGet]
	[ProducesResponseType(typeof(IEnumerable<BookingResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
	{
		var bookings = await _entityService.GetAllAsync(cancellationToken);
		return Ok(_mapper.Map<IEnumerable<BookingResponse>>(bookings));
	}

	/// <summary>
	/// Retrieves a specific Booking entity by its unique identifier.
	/// </summary>
	/// <param name="id">The ID of the Booking entity to retrieve.</param>
	/// <param name="cancellationToken">A CancellationToken.</param>
	/// <response code="200">Returns the Booking entity.</response>
	/// <response code="404">If the entity is not found.</response>
	/// <response code="401">If the user is unauthorized.</response>
	/// <response code="403">If the user is not the owner or an Admin.</response>
	[HttpGet("{id}")]
	[ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
	{
		var result = await _entityService.GetByIdAsync(id, cancellationToken);

		if (result.IsFailure)
			return result.ToActionResult();

		var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (result.Value!.UserId != currentUserId && !User.IsInRole(RoleList.Admin))
			return Result.Bad(BookingErrors.AccessDenied).ToActionResult();

		return Ok(_mapper.Map<BookingResponse>(result.Value));
	}

	/// <summary>
	/// Creates a new Booking entity.
	/// </summary>
	/// <param name="request">The request model for the new booking.</param>
	/// <response code="201">Returns the ID of the newly created booking.</response>
	/// <response code="400">Returns an Error object for validation failures (e.g., seat already booked, invalid session/user).</response>
	/// <response code="401">If the user is unauthorized.</response>
	[HttpPost]
	[ProducesResponseType(typeof(CreatedResponse<int>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
	[Authorize(Roles = RoleList.Customer)]
	public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
	{
		var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrEmpty(currentUserId))
			return Result.Bad(BookingErrors.UserIdMissing).ToActionResult();

		var bookingToCreate = _mapper.Map<Booking>(request);

		bookingToCreate.UserId = currentUserId;
		bookingToCreate.Status = BookingStatus.Pending;

		var result = await _entityService.CreateAsync(bookingToCreate);

		return result.Match(
			booking => Ok(new CreatedResponse<int>(booking.Id)),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Deletes a specific Booking entity by its unique identifier. (Admin only)
	/// </summary>
	/// <param name="id">The ID of the Booking entity to delete.</param>
	/// <response code="200">Indicates successful deletion.</response>
	/// <response code="404">Returns an Error object if the entity is not found.</response>
	/// <response code="401">If the user is unauthorized.</response>
	/// <response code="403">If the user is not an Admin.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[Authorize(Roles = RoleList.Admin)]
	public async Task<IActionResult> Delete(int id)
	{
		var result = await _entityService.DeleteByIdAsync(id);

		return result.Match(
			Ok,
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Checks if a specific seat is booked for a given session.
	/// </summary>
	/// <param name="sessionId">The ID of the Session.</param>
	/// <param name="rowNumber">The row number of the seat.</param>
	/// <param name="seatNumber">The seat number within the row.</param>
	/// <response code="200">Returns whether the seat is booked.</response>
	/// <response code="401">If the user is unauthorized.</response>
	[AllowAnonymous]
	[HttpGet("sessions/{sessionId}/seats/{rowNumber}/{seatNumber}/is-booked")]
	[ProducesResponseType(typeof(IsSeatBookedResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> IsSeatBooked(int sessionId, int rowNumber, int seatNumber)
	{
		bool isBooked = await _entityService.IsSeatBooked(sessionId, rowNumber, seatNumber);
		return Ok(new IsSeatBookedResponse(isBooked));
	}
}