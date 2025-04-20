using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers.V1.Genres.Requests;

public class UpdateGenreRequest
{
	[Required]
	[MaxLength(512)]
	public string Name { get; set; } = default!;
}