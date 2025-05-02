namespace Web.API.Controllers.V1.Contents.Responses;

public class UploadBannerResponse(string bannerUrl)
{
	public string BannerUrl { get; set; } = bannerUrl;
}