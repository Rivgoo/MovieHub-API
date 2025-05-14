using Application.Bookings;
using Application.Filters.Abstractions;
using Domain.Entities;
using Infrastructure.Core;
using Infrastructure.Filters.Abstractions;
using LinqKit;

namespace Infrastructure.Filters.Sorters;

internal class BookingSorter(CoreDbContext dbContext)
	: BaseSorter<Booking, BookingFilter>(dbContext), ISorter<Booking, BookingFilter>
{
	public override IQueryable<Booking> GetSort(BookingFilter filter)
	{
		var query = PredicateBuilder.New<Booking>(true);

		if (!string.IsNullOrWhiteSpace(filter.UserId))
			query = query.And(b => b.UserId == filter.UserId);

		if (filter.SessionId.HasValue)
			query = query.And(b => b.SessionId == filter.SessionId.Value);

		if (filter.Statuses.Count > 0)
			query = query.And(b => filter.Statuses.Contains(b.Status));

		if (filter.MinCreatedAt.HasValue)
			query = query.And(b => b.CreatedAt >= filter.MinCreatedAt.Value);

		if (filter.MaxCreatedAt.HasValue)
			query = query.And(b => b.CreatedAt < filter.MaxCreatedAt.Value.AddDays(1));

		bool needsSessionJoin = filter.ContentId.HasValue || filter.CinemaHallId.HasValue;

		if (filter.ContentId.HasValue)
			query = query.And(b => b.Session.ContentId == filter.ContentId.Value);

		if (filter.CinemaHallId.HasValue)
			query = query.And(b => b.Session.CinemaHallId == filter.CinemaHallId.Value);

		return _entities.AsExpandable().Where(query);
	}
}