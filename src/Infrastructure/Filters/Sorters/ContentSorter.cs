﻿using Application.Contents;
using Application.Filters.Abstractions;
using Domain.Entities;
using Infrastructure.Core;
using Infrastructure.Filters.Abstractions;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Filters.Sorters;

internal class ContentSorter(CoreDbContext dbContext)
	: BaseSorter<Content, ContentFilter>(dbContext), ISorter<Content, ContentFilter>
{
	public override IQueryable<Content> GetSort(ContentFilter filter)
	{
		var query = PredicateBuilder.New<Content>(true);

		if (!string.IsNullOrEmpty(filter.SearchTerms))
		{
			var terms = GetLikeTermsToSearch(filter.SearchTerms);

			var searchPredicate = PredicateBuilder.New<Content>(false);

			foreach (var term in terms)
				searchPredicate = searchPredicate.Or(c => EF.Functions.Like(c.Title, term) ||
														  EF.Functions.Like(c.Description, term) ||
														  EF.Functions.Like(c.DirectorFullName, term));

			query = query.And(searchPredicate);
		}

		if (filter.MinRating.HasValue)
			query = query.And(c => c.Rating != null && c.Rating >= filter.MinRating.Value);

		if (filter.MaxRating.HasValue)
			query = query.And(c => c.Rating != null && c.Rating <= filter.MaxRating.Value);

		if (filter.MinReleaseYear.HasValue)
			query = query.And(c => c.ReleaseYear >= filter.MinReleaseYear.Value);
		if (filter.MaxReleaseYear.HasValue)
			query = query.And(c => c.ReleaseYear <= filter.MaxReleaseYear.Value);

		if (filter.MinDurationMinutes.HasValue)
			query = query.And(c => c.DurationMinutes >= filter.MinDurationMinutes.Value);

		if (filter.MaxDurationMinutes.HasValue)
			query = query.And(c => c.DurationMinutes <= filter.MaxDurationMinutes.Value);

		if (filter.HasTrailer.HasValue)
			if (filter.HasTrailer.Value)
				query = query.And(c => !string.IsNullOrEmpty(c.TrailerUrl));
			else
				query = query.And(c => string.IsNullOrEmpty(c.TrailerUrl));

		if (filter.HasPoster.HasValue)
			if (filter.HasPoster.Value)
				query = query.And(c => !string.IsNullOrEmpty(c.PosterUrl));
			else
				query = query.And(c => string.IsNullOrEmpty(c.PosterUrl));

		if (filter.HasBanner.HasValue)
			query = query.And(filter.HasBanner.Value
				? c => !string.IsNullOrEmpty(c.BannerUrl)
				: c => string.IsNullOrEmpty(c.BannerUrl));

		if (filter.MinAgeRating.HasValue)
			query = query.And(c => c.AgeRating >= filter.MinAgeRating.Value);
		if (filter.MaxAgeRating.HasValue)
			query = query.And(c => c.AgeRating <= filter.MaxAgeRating.Value);

		if (filter.GenreIds.Count > 0)
			if (filter.MatchAllGenres)
				query = query.And(c => filter.GenreIds.All(gid => c.ContentGenres.Any(cg => cg.GenreId == gid)));
			else
				query = query.And(c => c.ContentGenres.Any(cg => filter.GenreIds.Contains(cg.GenreId)));

		if (filter.ActorIds.Count > 0)
			if (filter.MatchAllActors)
				query = query.And(c => filter.ActorIds.All(aid => c.ContentActors.Any(ca => ca.ActorId == aid)));
			else
				query = query.And(c => c.ContentActors.Any(ca => filter.ActorIds.Contains(ca.ActorId)));

		if (!string.IsNullOrEmpty(filter.FavoritedByUserId))
		{
			var userId = filter.FavoritedByUserId;

			query = query.And(c => c.FavoriteContents.Any(fc => fc.UserId == userId));
		}

		if (filter.HasSessions.HasValue)
			if (filter.HasSessions.Value)
			{
				query = query.And(c => c.Sessions.Any(s => s.StartTime > DateTime.UtcNow));

				if (filter.MinSessionStartTime.HasValue)
					query = query.And(c => c.Sessions.Any(s => s.StartTime >= filter.MinSessionStartTime.Value));

				if (filter.MaxSessionStartTime.HasValue)
					query = query.And(c => c.Sessions.Any(s => s.StartTime <= filter.MaxSessionStartTime.Value));
			}
			else
				query = query.And(c => !c.Sessions.Any());

		return _entities
			.AsExpandable()
			.Where(query);
	}
}