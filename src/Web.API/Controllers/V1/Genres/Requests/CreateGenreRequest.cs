using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers.V1.Genres.Requests;

public class CreateGenreRequest
{
	[Required]
	[MaxLength(512)]
	public string Name { get; set; } = default!;
}