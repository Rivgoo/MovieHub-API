using Application.CinemaHalls;
using Application.Filters.Abstractions;
using Domain.Entities;
using Infrastructure.Core;
using Infrastructure.Filters.Abstractions;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Filters.Sorters;

internal class CinemaHallSorter(CoreDbContext dbContext)
	: BaseSorter<CinemaHall, CinemaHallFilter>(dbContext), ISorter<CinemaHall, CinemaHallFilter>
{
	public override IQueryable<CinemaHall> GetSort(CinemaHallFilter filter)
	{
		var query = PredicateBuilder.New<CinemaHall>(true);
		var nameTerm = filter.Name?.Trim();

		if (!string.IsNullOrEmpty(nameTerm))
			query = query.And(ch => EF.Functions.Like(ch.Name, $"%{nameTerm}%"));

		if (filter.MinNumberOfRows.HasValue)
			query = query.And(ch => ch.SeatsPerRow.Count >= filter.MinNumberOfRows.Value);

		if (filter.MaxNumberOfRows.HasValue)
			query = query.And(ch => ch.SeatsPerRow.Count <= filter.MaxNumberOfRows.Value);

		return _entities
			.AsExpandable()
			.Where(query);
	}
}