namespace Web.API.Controllers.V1.Actors.Responses;

public class UploadActorPhotoResponse(string photoUrl)
{
	public string PhotoUrl { get; set; } = photoUrl;
}