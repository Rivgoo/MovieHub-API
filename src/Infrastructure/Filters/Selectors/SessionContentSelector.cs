using Application.Sessions.Abstractions;
using Application.Sessions.Dtos;
using Domain.Entities;

namespace Infrastructure.Filters;

internal class SessionContentSelector : ISessionContentSelector
{
	public IQueryable<SessionContentDto> Select(IQueryable<Session> source)
	{
		return source.Select(s => new SessionContentDto
		{
			Id = s.Id,
			StartTime = s.StartTime,
			ContentId = s.ContentId,
			CinemaHallId = s.CinemaHallId,
			Status = s.Status,
			TicketPrice = s.TicketPrice,
			Title = s.Content.Title,
			Rating = s.Content.Rating,
			ReleaseYear = s.Content.ReleaseYear,
			Description = s.Content.Description,
			PosterUrl = s.Content.PosterUrl,
			BannerUrl = s.Content.BannerUrl,
			TrailerUrl = s.Content.TrailerUrl,
			DurationMinutes = s.Content.DurationMinutes,
			GenreIds = s.Content.ContentGenres.Select(g => g.Id).ToList()
		});
	}
}