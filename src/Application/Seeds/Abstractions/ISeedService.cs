using Application.Results;

namespace Application.Seeds.Abstractions;

/// <summary>
/// Defines the contract for a service that seeds the database with fake data.
/// </summary>
public interface ISeedService
{
	/// <summary>
	/// Seeds the database with a specified number of fake entities and their relationships.
	/// </summary>
	/// <param name="contentCount">Number of Content items to create.</param>
	/// <param name="sessionsPerContent">Maximum number of Sessions per Content item.</param>
	/// <param name="genresCount">Number of Genres to ensure exist (create if needed).</param>
	/// <param name="actorsCount">Number of Actors to ensure exist (create if needed).</param>
	/// <param name="hallsCount">Number of CinemaHalls to ensure exist (create if needed).</param>
	/// <param name="customersCount">Number of Customer Users to ensure exist (create if needed).</param>
	/// <param name="maxBookingsPerSession">Maximum number of Bookings per Session.</param>
	/// <param name="uploadImages">Whether to generate and upload fake poster/photo images (can be slow).</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A Result indicating success or failure of the seeding operation.</returns>
	Task<Result> SeedDataAsync(
		int contentCount = 20,
		int sessionsPerContent = 5,
		int genresCount = 10,
		int actorsCount = 30,
		int hallsCount = 5,
		int customersCount = 500,
		int maxBookingsPerSession = 10,
		bool uploadImages = false,
		CancellationToken cancellationToken = default);
}