namespace Web.API.Controllers.V1.CinemaHalls.Responses;

public class CinemaHallResponse
{
	public int Id { get; set; }

	/// <summary>
	/// Gets the total capacity of the cinema hall, calculated from the seating layout.
	/// </summary>
	public int TotalCapacity => SeatsPerRow?.Sum() ?? 0;

	/// <summary>
	/// Gets the total number of rows in the cinema hall.
	/// </summary>
	public int NumberOfRows => SeatsPerRow?.Count ?? 0;

	/// <summary>
	/// Gets or sets the name of the cinema hall.
	/// </summary>
	public string Name { get; set; } = default!;

	/// <summary>
	/// Gets or sets the layout of seats per row.
	/// Each integer in the list represents the number of seats in the corresponding row.
	/// The index of the list represents the row number (0-based, usually treated as 1-based in display).
	/// </summary>
	public List<int> SeatsPerRow { get; set; } = [];
}