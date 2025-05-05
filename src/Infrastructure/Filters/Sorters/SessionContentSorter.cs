using Application.Filters.Abstractions;
using Application.Sessions;
using Domain.Entities;
using Infrastructure.Core;
using Infrastructure.Filters.Abstractions;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Filters.Sorters;

internal class SessionContentSorter(CoreDbContext dbContext)
	: BaseSorter<Session, SessionContentFilter>(dbContext), ISorter<Session, SessionContentFilter>
{
	public override IQueryable<Session> GetSort(SessionContentFilter filter)
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

		if (!string.IsNullOrEmpty(filter.SearchTerms))
		{
			var terms = GetLikeTermsToSearch(filter.SearchTerms);

			var searchPredicate = PredicateBuilder.New<Session>(false);

			foreach (var term in terms)
				searchPredicate = searchPredicate.Or(s => EF.Functions.Like(s.Content.Title, term) ||
														  EF.Functions.Like(s.Content.Description, term));

			query = query.And(searchPredicate);
		}

		if (filter.MinRating.HasValue)
			query = query.And(s => s.Content.Rating != null && s.Content.Rating >= filter.MinRating.Value);

		if (filter.MaxRating.HasValue)
			query = query.And(s => s.Content.Rating != null && s.Content.Rating <= filter.MaxRating.Value);

		if (filter.MinReleaseYear.HasValue)
			query = query.And(s => s.Content.ReleaseYear >= filter.MinReleaseYear.Value);
		if (filter.MaxReleaseYear.HasValue)
			query = query.And(s => s.Content.ReleaseYear <= filter.MaxReleaseYear.Value);

		if (filter.MinDurationMinutes.HasValue)
			query = query.And(s => s.Content.DurationMinutes >= filter.MinDurationMinutes.Value);

		if (filter.MaxDurationMinutes.HasValue)
			query = query.And(s => s.Content.DurationMinutes <= filter.MaxDurationMinutes.Value);

		if (filter.HasTrailer.HasValue)
			if (filter.HasTrailer.Value)
				query = query.And(s => !string.IsNullOrEmpty(s.Content.TrailerUrl));
			else
				query = query.And(s => string.IsNullOrEmpty(s.Content.TrailerUrl));

		if (filter.HasPoster.HasValue)
			if (filter.HasPoster.Value)
				query = query.And(s => !string.IsNullOrEmpty(s.Content.PosterUrl));
			else
				query = query.And(s => string.IsNullOrEmpty(s.Content.PosterUrl));

		if (filter.HasBanner.HasValue)
			query = query.And(filter.HasBanner.Value
				? s => !string.IsNullOrEmpty(s.Content.BannerUrl)
				: s => string.IsNullOrEmpty(s.Content.BannerUrl));

		if (filter.GenreIds.Count > 0)
			if (filter.MatchAllGenres)
				query = query.And(s => filter.GenreIds.All(gid => s.Content.ContentGenres.Any(cg => cg.GenreId == gid)));
			else
				query = query.And(s => s.Content.ContentGenres.Any(cg => filter.GenreIds.Contains(cg.GenreId)));

		return _entities
			.Include(s => s.CinemaHall)
			.AsExpandable()
			.Where(query);
	}
}