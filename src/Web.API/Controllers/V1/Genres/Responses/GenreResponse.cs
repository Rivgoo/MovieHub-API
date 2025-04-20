namespace Web.API.Controllers.V1.Genres.Responses;

public class GenreResponse
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}