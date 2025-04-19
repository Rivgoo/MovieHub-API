namespace Web.API.Core.Options;

public class InitialAdminOptions
{
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public string PhoneNumber { get; set; } = string.Empty;
}