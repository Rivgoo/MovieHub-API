namespace Web.API.Core.Jwt;

/// <summary>
/// Configuration options for JSON Web Token (JWT) authentication and token generation.
/// </summary>
/// <remarks>
/// This class is typically used to bind configuration settings from sources like
/// appsettings.json to strongly-typed properties, providing essential parameters
/// for JWT token issuance and validation.
/// </remarks>
public class JwtOptions
{
	/// <summary>
	/// Gets or sets the secret key used for signing the JWT token.
	/// </summary>
	/// <value>
	/// A string representing the secret key. This key is crucial for verifying the
	/// token's signature and must be kept confidential on the server side.
	/// </value>
	public string Secret { get; set; } = default!;

	/// <summary>
	/// Gets or sets the issuer of the JWT token.
	/// </summary>
	/// <value>
	/// A string identifying the principal that issued the JWT (e.g., your application's URL).
	/// This is typically included in the 'iss' claim of the token.
	/// </value>
	public string Issuer { get; set; } = default!;

	/// <summary>
	/// Gets or sets the intended audience(s) of the JWT token.
	/// </summary>
	/// <value>
	/// A string or array of strings identifying the recipient(s) the JWT is intended for.
	/// This is typically included in the 'aud' claim of the token.
	/// </value>
	public string Audience { get; set; } = default!;

	/// <summary>
	/// Gets or sets the expiration time for the access token, specified in minutes.
	/// </summary>
	/// <value>
	/// An integer representing the duration in minutes after which the access token expires.
	/// </value>
	public int AccessExpirationInMinutes { get; set; }
}