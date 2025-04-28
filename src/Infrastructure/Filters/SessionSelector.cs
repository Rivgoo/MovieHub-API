using Application.Sessions.Abstractions;
using Application.Sessions.Models;
using Domain.Entities;

namespace Infrastructure.Filters;

internal class SessionSelector : ISessionSelector
{
	public IQueryable<SessionDto> Select(IQueryable<Session> source)
	{
		return source.Select(s => new SessionDto
		{
			Id = s.Id,
			StartTime = s.StartTime,
			ContentId = s.ContentId,
			CinemaHallId = s.CinemaHallId,
			Status = s.Status,
			TicketPrice = s.TicketPrice
		});
	}
}