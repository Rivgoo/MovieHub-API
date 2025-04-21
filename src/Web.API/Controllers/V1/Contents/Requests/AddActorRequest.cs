using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers.V1.Contents.Requests;

public class AddActorRequest
{
	[Required]
	public int ActorId { get; set; }

	[Required]
	[MaxLength(255)]
	public string RoleName { get; set; }
}