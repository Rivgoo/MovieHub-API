namespace Domain.Enums;

/// <summary>
/// Represents the status of a booking.
/// </summary>
[Flags]
public enum BookingStatus
{
	/// <summary>
	/// The booking is pending and awaiting confirmation.
	/// </summary>
	Pending,

	/// <summary>
	/// The booking has been confirmed.
	/// </summary>
	Confirmed,

	/// <summary>
	/// The booking has been canceled.
	/// </summary>
	Canceled
}