using Application.Results;
using Domain.Entities;

namespace Application.Sessions;

public class SessionErrors : EntityErrors<Session, int>
{
	public static Error InvalidTicketPrice => Error.BadRequest(
		$"{EntityName}.{nameof(InvalidTicketPrice)}",
		"Ticket price must be greater than zero and less then 100000.");

	public static Error InvalidStartTime => Error.BadRequest(
		$"{EntityName}.{nameof(InvalidStartTime)}",
		"Start time must be in the future.");
}