using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers.V1.Authentications;

public class AuthenticationRequest
{
	[Required]
	[MaxLength(255)]
	public string Email { get; set; }

	[Required]
	[MaxLength(255)]
	public string Password { get; set; }
}