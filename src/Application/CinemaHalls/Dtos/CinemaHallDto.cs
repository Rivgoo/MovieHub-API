namespace Application.CinemaHalls.Dtos;

/// <summary>
/// Data Transfer Object for Cinema Hall information.
/// </summary>
public class CinemaHallDto
{
	/// <summary>
	/// Gets or sets the unique identifier for the cinema hall.
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// Gets or sets the name of the cinema hall.
	/// </summary>
	public string Name { get; set; } = default!;

	/// <summary>
	/// Gets or sets the layout of seats per row.
	/// </summary>
	public List<int> SeatsPerRow { get; set; } = [];

	/// <summary>
	/// Gets the total number of rows in the cinema hall.
	/// </summary>
	public int NumberOfRows { get; set; } 

	/// <summary>
	/// Gets the total capacity of the cinema hall.
	/// </summary>
	public int TotalCapacity { get; set; }

	/// <summary>
	/// Gets or sets the creation timestamp.
	/// </summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>
	/// Gets or sets the last update timestamp.
	/// </summary>
	public DateTime UpdatedAt { get; set; }
}