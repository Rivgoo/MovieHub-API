using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers.V1.Contents.Requests;

public class UploadBannerRequest
{
	[Required]
	public string Base64Image { get; set; } = default!;
}