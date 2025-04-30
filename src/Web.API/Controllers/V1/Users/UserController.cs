using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.API.Core.BaseResponses;
using Web.API.Core;
using AutoMapper;
using Application.Users.Abstractions;
using Application.Users.Models;
using Application.Results;
using Domain;
using System.Security.Claims;

namespace Web.API.Controllers.V1.Users;

/// <summary>
/// API Controller for managing User entities and handling user registration.
/// </summary>
/// <param name="mapper">The AutoMapper instance for object mapping.</param>
/// <param name="entityService">The service for managing User entities (<see cref="IUserService"/>).</param>
/// <param name="userRegistrator">The service for registering new users (<see cref="IUserRegistrator"/>).</param>
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/users")]
public class UserController(
	IMapper mapper,
	IUserService entityService,
	IUserRegistrator userRegistrator) :
	EntityApiController<IUserService>(mapper, entityService)
{
	private readonly IUserRegistrator _userRegistrator = userRegistrator;

	/// <summary>
	/// Checks if a User entity with the specified ID exists.
	/// </summary>
	/// <param name="id">The ID of the User entity to check.</param>
	/// <returns>An <see cref="IActionResult"/> representing the response indicating whether the User entity exists.</returns>
	/// <response code="200">Returns <see cref="ExistsResponse"/> with <see langword="true"/> if the entity exists, or <see langword="false"/> otherwise.</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpGet("{id}/exists")]
	[ProducesResponseType(typeof(ExistsResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> ExistsById(string id)
		=> Ok(new ExistsResponse(await _entityService.ExistsByIdAsync(id)));

	/// <summary>
	/// Retrieves basic information for a specific user by their ID.
	/// </summary>
	/// <param name="id">The unique identifier of the user.</param>
	/// <response code="200">Returns the user's basic information.</response>
	/// <response code="404">If a user with the specified ID is not found.</response>
	/// <response code="401">If the client is not authorized to perform this action.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpGet("{id}/info")]
	[ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> GetInfoById(string id)
		=> (await _entityService.GetUserInfoByIdAsync(id)).ToActionResult();

	/// <summary>
	/// Retrieves the basic information of the currently authenticated user.
	/// </summary>
	/// <response code="200">Returns the user's basic information.</response>
	/// <response code="404">If a user is not found.</response>
	/// <response code="401">If the client is not authorized to perform this action.</response>
	[HttpGet("/info")]
	[ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> GetInfoByToken()
	{
		var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (string.IsNullOrEmpty(id))
			return Unauthorized(Error.AccessForbidden("User.NotFound", "User not found"));

		return (await _entityService.GetUserInfoByIdAsync(id)).ToActionResult();
	}

	/// <summary>
	/// Registers a new user with the 'Admin' role.
	/// </summary>
	/// <param name="request">The request model (<see cref="RegistrationUserRequest"/>) containing the details for the new admin user.</param>
	/// <returns>An <see cref="IActionResult"/> representing the registration result.</returns>
	/// <response code="200">Returns response containing the ID of the newly created admin user.</response>
	/// <response code="400">Returns error for validation failures (e.g., invalid password, invalid phone number) or if a user with the provided email already exists</response>
	/// <response code="401">If the request does not contain a valid authentication token.</response>
	/// <response code="403">If the authenticated user does not have the '<c>Admin</c>' role.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpPost("admins/register")]
	[ProducesResponseType(typeof(CreatedResponse<string>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> RegisterAdmin([FromBody] RegistrationUserRequest request)
	{
		var model = _mapper.Map<RegistrationUserModel>(request);
		var result = await _userRegistrator.RegisterAdminAsync(model);

		return result.Match(
			_ => Ok(new CreatedResponse<string>(result.Value!.Id)),
			error => result.ToActionResult());
	}

	/// <summary>
	/// Registers a new user with the 'Customer' role.
	/// </summary>
	/// <param name="request">The request model (<see cref="RegistrationUserRequest"/>) containing the details for the new customer user.</param>
	/// <returns>An <see cref="IActionResult"/> representing the registration result.</returns>
	/// <response code="200">Returns response containing the ID of the newly created customer user.</response>
	/// <response code="400">Returns error for validation failures (e.g., invalid phone number) or if a user with the provided email already exists.</response>
	[AllowAnonymous]
	[HttpPost("customer/register")]
	[ProducesResponseType(typeof(CreatedResponse<string>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	public async Task<IActionResult> RegisterCustomer([FromBody] RegistrationUserRequest request)
	{
		var model = _mapper.Map<RegistrationUserModel>(request);
		var result = await _userRegistrator.RegisterCustomerAsync(model);

		return result.Match(
			_ => Ok(new CreatedResponse<string>(result.Value!.Id)),
			error => result.ToActionResult());
	}
}