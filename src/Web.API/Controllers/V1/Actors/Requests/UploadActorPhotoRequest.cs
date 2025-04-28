using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers.V1.Actors.Requests;

public class UploadActorPhotoRequest
{
	[Required]
	public string Base64Image { get; set; } = default!;
}