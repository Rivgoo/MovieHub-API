using Application.Filters.Abstractions;
using Application.Sessions;
using Domain.Entities;
using Infrastructure.Core;
using Infrastructure.Filters.Abstractions;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Filters.Sorters;

internal class SessionSorter(CoreDbContext dbContext)
	: BaseSorter<Session, SessionFilter>(dbContext), ISorter<Session, SessionFilter>
{
	public override IQueryable<Session> GetSort(SessionFilter filter)
	{
		var query = PredicateBuilder.New<Session>(true);

		if (filter.MinStartTime.HasValue)
			query = query.And(s => s.StartTime >= filter.MinStartTime.Value);

		if (filter.MaxStartTime.HasValue)
			query = query.And(s => s.StartTime <= filter.MaxStartTime.Value);

		if (filter.ContentId.HasValue)
			query = query.And(s => s.ContentId == filter.ContentId.Value);

		if (filter.CinemaHallId.HasValue)
			query = query.And(s => s.CinemaHallId == filter.CinemaHallId.Value);

		if (filter.Status.HasValue)
			query = query.And(s => s.Status == filter.Status.Value);

		if (filter.MinTicketPrice.HasValue)
			query = query.And(s => s.TicketPrice >= filter.MinTicketPrice.Value);

		if (filter.MaxTicketPrice.HasValue)
			query = query.And(s => s.TicketPrice <= filter.MaxTicketPrice.Value);

		if (filter.HasAvailableSeats.HasValue && filter.HasAvailableSeats.Value)
			query = query.And(s => s.Bookings.Count < s.CinemaHall.TotalCapacity);

		return _entities
			.Include(s => s.CinemaHall)
			.AsExpandable()
			.Where(query);
	}
}