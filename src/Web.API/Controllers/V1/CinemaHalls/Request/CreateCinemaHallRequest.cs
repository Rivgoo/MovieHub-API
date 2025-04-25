using System.ComponentModel.DataAnnotations;

namespace Web.API.Controllers.V1.CinemaHalls.Request;

public class CreateCinemaHallRequest
{
	/// <summary>
	/// Gets or sets the name of the cinema hall.
	/// </summary>
	[Required]
	[MaxLength(512)]
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the layout of seats per row.
	/// Each integer in the list represents the number of seats in the corresponding row.
	/// The index of the list represents the row number (0-based, usually treated as 1-based in display).
	/// </summary>
	/// <value>A list of integers where list[i] is the number of seats in row i+1.</value>
	[Required]
	public List<int> SeatsPerRow { get; set; }
}