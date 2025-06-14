﻿using Domain.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

/// <summary>
/// Represents a cinema hall within the application.
/// </summary>
public class CinemaHall : BaseEntity<int>, IBaseEntity<int>
{
	/// <summary>
    /// Gets the total capacity of the cinema hall.
    /// </summary>
    public int TotalCapacity { get; set; }

	/// <summary>
	/// Gets the total number of rows in the cinema hall.
	/// </summary>
	public int NumberOfRows { get; set; }

	/// <summary>
	/// Gets or sets the name of the cinema hall.
	/// </summary>
	[Required]
	[MaxLength(512)]
	public string Name { get; set; } = default!;

	/// <summary>
	/// Gets or sets the layout of seats per row.
	/// Each integer in the list represents the number of seats in the corresponding row.
	/// The index of the list represents the row number (0-based, usually treated as 1-based in display).
	/// </summary>
	/// <value>A list of integers where list[i] is the number of seats in row i+1.</value>
	[Required]
	[Column(TypeName = "json")]
	public List<int> SeatsPerRow { get; set; } = [];

	/// <summary>
	/// Gets or sets sessions associated with the cinema hall.
	/// </summary>
	public ICollection<Session> Sessions { get; set; } = default!;
}