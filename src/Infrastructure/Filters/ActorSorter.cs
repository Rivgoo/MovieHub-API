using Application.Actors;
using Application.Filters.Abstractions;
using Domain.Entities;
using Infrastructure.Core;
using Infrastructure.Filters.Abstractions;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Filters;

internal class ActorSorter(CoreDbContext dbContext)
	: BaseSorter<Actor, ActorFilter>(dbContext), ISorter<Actor, ActorFilter>
{
	public override IQueryable<Actor> GetSort(ActorFilter filter)
	{
		var query = PredicateBuilder.New<Actor>(true);

		if (filter.HasPhoto.HasValue)
			if (filter.HasPhoto.Value)
				query = query.And(a => !string.IsNullOrEmpty(a.PhotoUrl));
			else
				query = query.And(a => string.IsNullOrEmpty(a.PhotoUrl));

		if (filter.ContentId.HasValue)
			query = query.And(a => a.ContentActors.Any(ca => ca.ContentId == filter.ContentId.Value));

		if (!string.IsNullOrEmpty(filter.SearchTerms))
		{
			var terms = GetLikeTermsToSearch(filter.SearchTerms);

			var searchPredicate = PredicateBuilder.New<Actor>(false);

			foreach (var term in terms)
				searchPredicate = searchPredicate.Or(c => EF.Functions.Like(c.FirstName, term) ||
														  EF.Functions.Like(c.LastName, term));

			query = query.And(searchPredicate);
		}

		return _entities
			.AsExpandable()
			.Where(query);
	}
}