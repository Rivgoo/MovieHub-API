namespace Web.API.Controllers.V1.Contents.Responses;

public class UploadPosterResponse(string posterUrl)
{
	public string PosterUrl { get; set; } = posterUrl;
}