using Domain.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Represents a cinema hall within the application.
/// </summary>
public class CinemaHall : BaseEntity<int>, IBaseEntity<int>
{
	/// <summary>
	/// Gets or sets the name of the cinema hall.
	/// </summary>
	[Required]
	[MaxLength(512)]
	public string Name { get; set; } = default!;

	/// <summary>
	/// Gets or sets the location of the cinema hall.
	/// </summary>
	[Required]
	public int Capacity { get; set; }

	/// <summary>
	/// Gets or sets sessions associated with the cinema hall.
	/// </summary>
	public ICollection<Session> Sessions { get; set; } = default!;
}