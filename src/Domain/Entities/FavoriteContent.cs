using Domain.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Represents a user's favorite content.
/// </summary>
public class FavoriteContent : BaseEntity<int>, IBaseEntity<int>
{
	/// <summary>
	/// Gets or sets the unique identifier for the user.
	/// </summary>
	[Required]
	public string UserId { get; set; } = default!;

	/// <summary>
	/// Gets or sets the unique identifier for the content.
	/// </summary>
	[Required]
	public int ContentId { get; set; } = default!;

	/// <summary>
	/// Gets or sets the user associated with this favorite content.
	/// </summary>
	public User User { get; set; } = default!;

	/// <summary>
	/// Gets or sets the content associated with this favorite content.
	/// </summary>
	public Content Content { get; set; } = default!;
}