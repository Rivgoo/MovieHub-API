using Application.Results;
using Application.Users.Abstractions;
using Application.Users.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.API.Core;
using Web.API.Core.Jwt;

namespace Web.API.Controllers.V1.Authentications;

/// <summary>
/// API Controller for handling user authentication and token issuance.
/// </summary>
/// <remarks>
/// This controller provides endpoints for user login to obtain JWT and refresh tokens.
/// It interacts with user services to authenticate credentials and JWT services
/// to generate tokens based on the configured policy.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="AuthenticationController"/> class.
/// </remarks>
/// <param name="jwtAuthentication">The JWT authentication service for token operations.</param>
/// <param name="userService">The user service for user authentication logic.</param>
/// <param name="configuration">The application configuration.</param>
[ApiVersion("1")]
[ApiController]
[Route("api/v{version:apiVersion}/auth")]
public class AuthenticationController(
	JwtAuthentication jwtAuthentication,
	IUserService userService,
	IConfiguration configuration) : ApiController 
{
	private readonly JwtAuthentication _jwtAuthentication = jwtAuthentication; 
	private readonly IConfiguration _configuration = configuration;
	private readonly IUserService _userService = userService;
	private const string _enableAPIAuthKey = "EnableAPIAuth";

	/// <summary>
	/// Authenticates a user and issues a JWT access token upon successful login.
	/// </summary>
	/// <remarks>
	/// This endpoint handles user login requests. It first checks if API authentication
	/// is enabled via configuration. If enabled, it attempts to authenticate the user
	/// using the provided email and password via the user service. Upon successful
	/// authentication, it generates a JWT access token.
	///
	/// Authentication failures (e.g., invalid credentials, locked out) are communicated
	/// via HTTP <see cref="StatusCodes.Status400BadRequest"/> with an <see cref="AuthenticationResult"/>
	/// object in the response body where <see cref="AuthenticationResult.Succeeded"/> is <see langword="false"/>
	/// and specific failure flags are set.
	///
	/// This endpoint does NOT require authentication itself.
	/// </remarks>
	/// <param name="request">The authentication request containing user credentials (email and password).</param>
	/// <response code="200">Returns the AuthenticationResult with user information and tokens upon successful authentication.</response>
	/// <response code="400">Returns the AuthenticationResult with AuthenticationResult.Succeeded set to false and details about the authentication failure (e.g., invalid credentials, locked out).</response>
	/// <response code="401">Returns an Error object if API authentication is disabled via configuration or if the request is otherwise unauthorized.</response>
	[HttpPost()]
	[AllowAnonymous]
	[ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Token([FromBody] AuthenticationRequest request)
	{
		if (!_configuration.GetValue<bool>(_enableAPIAuthKey))
			return Result.Bad(Error.Unauthorized("API.Disabled", "API authentication is disabled.")).ToActionResult();

		var result = await _userService.TryAuthentication(request.Email, request.Password);

		if (!result.Succeeded)
			return BadRequest(result);

		result.AccessToken = _jwtAuthentication.GenerateToken(result.User!.Id, result.User.Role);

		return Ok(result);
	}
}