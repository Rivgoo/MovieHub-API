using Application.Filters.Abstractions;
using Domain.Entities;

namespace Application.Users;

public class UserFilter : BaseFilter<User>
{
	/// <summary>
	/// Filter by first name (case-insensitive, partial match).
	/// </summary>
	public string? FirstName { get; set; }

	/// <summary>
	/// Filter by last name (case-insensitive, partial match).
	/// </summary>
	public string? LastName { get; set; }

	/// <summary>
	/// Filter by email address (case-insensitive, partial match).
	/// </summary>
	public string? Email { get; set; }

	/// <summary>
	/// Filter by user name (case-insensitive, partial match).
	/// </summary>
	public string? UserName { get; set; }

	/// <summary>
	/// Filter by phone number (case-insensitive, partial match).
	/// </summary>
	public string? PhoneNumber { get; set; }

	/// <summary>
	/// Filter users based on whether their account is blocked.
	/// true: only blocked users.
	/// false: only non-blocked users.
	/// null: ignore this filter.
	/// </summary>
	public bool? IsBlocked { get; set; }

	/// <summary>
	/// Filter users based on whether their email is confirmed.
	/// true: only confirmed emails.
	/// false: only non-confirmed emails.
	/// null: ignore this filter.
	/// </summary>
	public bool? EmailConfirmed { get; set; }

	/// <summary>
	/// Filter by minimum creation date (inclusive).
	/// </summary>
	public DateTime? MinCreatedAt { get; set; }

	/// <summary>
	/// Filter by maximum creation date (inclusive).
	/// </summary>
	public DateTime? MaxCreatedAt { get; set; }

	/// <summary>
	/// Filter users by a specific role id.
	/// </summary>
	public string? RoleId { get; set; }
}