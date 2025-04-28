using Application.Filters.Abstractions;
using Domain.Entities;

namespace Application.Actors;

public class ActorFilter : BaseFilter<Actor>
{
	public string? SearchTerms { get; set; }

	/// <summary>
	/// Filter actors based on whether they have a photo uploaded.
	/// true: only actors with a PhotoUrl.
	/// false: only actors without a PhotoUrl.
	/// null: ignore this filter.
	/// </summary>
	public bool? HasPhoto { get; set; }

	/// <summary>
	/// Filter actors by the ID of the Content they starred in.
	/// </summary>
	public int? ContentId { get; set; }
}