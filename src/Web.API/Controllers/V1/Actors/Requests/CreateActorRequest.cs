using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers.V1.Actors.Requests;

public class CreateActorRequest
{
	[Required]
	[MaxLength(255)]
	public string FirstName { get; set; } = default!;

	[Required]
	[MaxLength(255)]
	public string LastName { get; set; } = default!;
}