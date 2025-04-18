namespace Domain.Enums;

/// <summary>
/// Represents the status of a session.
/// </summary>
[Flags]
public enum SessionStatus
{
	/// <summary>
	/// The session is currently ongoing.
	/// </summary>
	Ongoing,

	/// <summary>
	/// The session has ended.
	/// </summary>
	Ended,

	/// <summary>
	/// The session is scheduled to start in the future.
	/// </summary>
	Scheduled
}