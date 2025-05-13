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
using Application.Filters.Abstractions;
using Application.Users;
using Domain.Entities;
using Application.Filters;
using Application.Users.Dtos;
using Web.API.Controllers.V1.Users.Requests;
using Application.Contents.Abstractions.Services;
using Web.API.Controllers.V1.Authentications;
using Application.Roles.Dtos;
using Microsoft.AspNetCore.Identity;

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
	IUserRegistrator userRegistrator,
	IFilterService<User, UserFilter> filterService,
	IFavoriteContentService favoriteContentService,
	RoleManager<Role> roleManager) :
	EntityApiController<IUserService>(mapper, entityService)
{
	private readonly IUserRegistrator _userRegistrator = userRegistrator;
	private readonly IFilterService<User, UserFilter> _filterService = filterService;
	private readonly IFavoriteContentService _favoriteContentService = favoriteContentService;
	private readonly RoleManager<Role> _roleManager = roleManager;

	/// <summary>
	/// Retrieves user items based on filter, pagination, and ordering criteria. (Admin only)
	/// </summary>
	/// <param name="pageSize">The number of items to return per page.</param>
	/// <param name="orderField">The field name(s) to order by (e.g., "Email", "LastName", "CreatedAt").</param>
	/// <param name="orderType">The order type(s) corresponding to each orderField.</param>
	/// <param name="filter">The filter criteria object.</param>
	/// <response code="200">Returns the paginated list of user DTOs, including their roles.</response>
	/// <response code="400">Returns an error if the input is invalid.</response>
	/// <response code="401">If the user is unauthorized.</response>
	/// <response code="403">If the user is not an Admin.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpGet("filter")]
	[ProducesResponseType(typeof(PaginatedList<UserDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> ByFilter(int pageSize,
		[FromQuery] string[] orderField, [FromQuery] List<QueryableOrderType> orderType, [FromQuery] UserFilter filter)
	{
		if (orderField == null || orderField.Length == 0)
		{
			orderField = ["Email"];
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

		var filterResult = await _filterService.SetPageSize(pageSize)
			.AddFilter(filter)
			.ApplyAsync<UserDto, IUserSelector>();

		return filterResult.ToActionResult();
	}

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
	/// Retrieves a specific User entity by its unique identifier. (Admin only)
	/// </summary>
	/// <param name="id">The ID of the User entity to retrieve.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <response code="200">Returns the <c>UserDto</c> for the specified user.</response>
	/// <response code="404">If the user with the specified ID is not found.</response>
	/// <response code="401">If the user is not authenticated.</response>
	/// <response code="403">If the authenticated user does not have the 'Admin' role.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpGet("{id}")]
	[ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
	{
		return (await _entityService.GetDtoByIdAsync(id, cancellationToken)).ToActionResult();
	}

	/// <summary>
	/// Retrieves the basic information of the currently authenticated user.
	/// </summary>
	/// <response code="200">Returns the user's basic information.</response>
	/// <response code="404">If a user is not found.</response>
	/// <response code="401">If the client is not authorized to perform this action.</response>
	[HttpGet("my-info")]
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
	/// Retrieves a list of all available roles in the system. (Admin only)
	/// </summary>
	/// <response code="200">Returns a list of all roles with their IDs and names.</response>
	/// <response code="401">If the user is not authenticated.</response>
	/// <response code="403">If the authenticated user does not have the 'Admin' role.</response>
	[Authorize(Roles = RoleList.Admin)]
	[HttpGet("roles")]
	[ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetAllRoles()
	{
		var roles = _roleManager.Roles.ToList();
		var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);

		return Ok(roleDtos);
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

	/// <summary>
	/// Updates an existing User entity.
	/// </summary>
	/// <summary>
	/// Updates the user with the matching ID using the provided data.
	/// Requires authentication.
	/// </summary>
	/// <param name="id">The ID of the User entity to update.</param>
	/// <param name="request">The request model (<c>UpdateUserRequest</c>) with updated data.</param>
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
	public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequest request)
	{
		var userToUpdate = _mapper.Map<User>(request);
		userToUpdate.Id = id;

		var result = await _entityService.UpdateAsync(userToUpdate);

		return result.Match(
			_ => Ok(),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Deletes a specific User entity by its unique identifier.
	/// </summary>
	/// <summary>
	/// Deletes the user with the matching ID.
	/// Requires authentication.
	/// </summary>
	/// <param name="id">The ID of the User entity to delete.</param>
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
	public async Task<IActionResult> Delete(string id)
	{
		var result = await _entityService.DeleteByIdAsync(id);

		return result.Match(
			Ok,
			error => result.ToActionResult()
		);
	}

	#region Favorite Content
	/// <summary>
	/// Adds a specified content to the current user's favorites.
	/// </summary>
	/// <param name="contentId">The ID of the content to add to favorites.</param>
	/// <response code="200">Content successfully added to favorites.</response>
	/// <response code="400">If the content is already in favorites, or other validation error occurs (e.g. content or user not found).</response>
	/// <response code="401">If the user is not authenticated.</response>
	/// <response code="404">If the user or content related to the operation does not exist.</response>
	/// <response code="409">If the content is already in favorites.</response>
	[Authorize]
	[HttpPost("favorites/{contentId:int}")]
	[ProducesResponseType(typeof(CreatedResponse<int>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> AddToFavorites(int contentId)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrEmpty(userId))
			return Unauthorized(AuthenticationErrors.InvalidCredentials);

		var result = await favoriteContentService.CreateAsync(userId, contentId);

		return result.Match(
			_ => Ok(new CreatedResponse<int>(result.Value!.Id)),
			error => result.ToActionResult()
		);
	}

	/// <summary>
	/// Removes a specified content from the current user's favorites.
	/// </summary>
	/// <param name="contentId">The ID of the content to remove from favorites.</param>
	/// <response code="200">Content successfully removed from favorites (or was not favorited).</response>
	/// <response code="401">If the user is not authenticated.</response>
	/// <response code="404">If the user or content related to the operation does not exist (less likely if remove is idempotent on favorite entry).</response>
	[Authorize]
	[HttpDelete("favorites/{contentId:int}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> RemoveFromFavorites(int contentId)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrEmpty(userId))
			return Unauthorized(AuthenticationErrors.InvalidCredentials);

		var result = await favoriteContentService.DeleteAsync(userId, contentId);
		return result.ToActionResult();
	}

	/// <summary>
	/// Checks if a specified content is in the current user's favorites.
	/// </summary>
	/// <param name="contentId">The ID of the content to check.</param>
	/// <param name="cancellationToken">A CancellationToken.</param>
	/// <response code="200">Returns a boolean indicating if the content is in favorites.</response>
	/// <response code="401">If the user is not authenticated.</response>
	/// <response code="404">If the user or content does not exist.</response>
	[Authorize]
	[HttpGet("favorites/{contentId:int}/exists")]
	[ProducesResponseType(typeof(ExistsResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> IsContentInFavorites(int contentId, CancellationToken cancellationToken)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrEmpty(userId))
			return Unauthorized(AuthenticationErrors.InvalidCredentials);

		var result = await _favoriteContentService.IsFavoriteAsync(userId, contentId, cancellationToken);

		return result.Match(
			isFavorite => Ok(new ExistsResponse(isFavorite)),
			error => result.ToActionResult()
		);
	}
	#endregion
}