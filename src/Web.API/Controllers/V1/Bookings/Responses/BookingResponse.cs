using Domain.Enums;

namespace Web.API.Controllers.V1.Bookings.Responses;

public class BookingResponse
{
	public int Id { get; set; }
	public string UserId { get; set; } = default!;
	public int SessionId { get; set; }
	public int RowNumber { get; set; }
	public int SeatNumber { get; set; }
	public BookingStatus Status { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}