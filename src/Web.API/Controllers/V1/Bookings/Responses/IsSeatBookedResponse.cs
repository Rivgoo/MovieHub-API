namespace Web.API.Controllers.V1.Bookings.Responses;

/// <summary>
/// Response indicating if a specific seat is booked.
/// </summary>
/// <param name="IsBooked">True if the seat is booked, false otherwise.</param>
public record IsSeatBookedResponse(bool IsBooked);