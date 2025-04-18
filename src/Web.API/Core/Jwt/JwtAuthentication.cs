using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Duende.IdentityModel;

namespace Web.API.Core.Jwt;

/// <summary>
/// Provides JWT-based authentication and token generation functionalities.
/// </summary>
/// <remarks>
/// This class is responsible for validating user credentials (mocked in this example)
/// and generating JWT tokens containing user identity and role claims based on configured options.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="JwtAuthentication"/> class.
/// </remarks>
/// <param name="configuration">The application configuration, used to retrieve JWT settings.</param>
public class JwtAuthentication(IConfiguration configuration)
{
	private readonly IConfiguration _configuration = configuration;

	/// <summary>
	/// Generates a JWT token for the specified user, including identity and role claims.
	/// </summary>
	/// <param name="userId">The unique identifier of the user (e.g., IdentityUser.Id).</param>
	/// <param name="role">The list of roles assigned to the user.</param> 
	/// <returns>A JWT token as a string.</returns>
	/// <exception cref="ArgumentNullException">Thrown if JWT options are not found in configuration.</exception>
	public string GenerateToken(string userId, string role)
	{
		var jwtSettings = _configuration.GetSection("Jwt").Get<JwtOptions>();

		if (jwtSettings == null)
			throw new ArgumentNullException(nameof(jwtSettings), "JWT options are not configured.");

		var claims = new List<Claim>
		{
			new(JwtClaimTypes.Subject, userId),
			new(JwtClaimTypes.Role, role),
			new(JwtClaimTypes.JwtId, Guid.NewGuid().ToString()),
			new(JwtClaimTypes.IssuedAt, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var expires = DateTime.UtcNow.AddMinutes(jwtSettings.AccessExpirationInMinutes);

		var token = new JwtSecurityToken(
			issuer: jwtSettings.Issuer,
			audience: jwtSettings.Audience,
			claims: claims,
			notBefore: DateTime.UtcNow,
			expires: expires,
			signingCredentials: creds);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}