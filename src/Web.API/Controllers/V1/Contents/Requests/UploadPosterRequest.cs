using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers.V1.Contents.Requests;

public class UploadPosterRequest
{
	[Required]
	public string Base64Image { get; set; } = default!;
}