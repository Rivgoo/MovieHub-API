using Application.Abstractions.Services;
using Application.Actors.Abstractions;
using Application.Bookings.Abstractions;
using Application.Bookings;
using Application.CinemaHalls.Abstractions;
using Application.Contents.Abstractions.Services;
using Application.Genres.Abstractions;
using Application.Results;
using Application.Seeds.Abstractions;
using Application.Sessions.Abstractions;
using Application.Users.Abstractions;
using Application.Users.Models;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Bogus;

namespace Application.Seeds;

/// <summary>
/// Service responsible for seeding the database with fake data.
/// Uses other entity services and IUserRegistrator.
/// </summary>
internal class SeedService(
	ILogger<SeedService> logger,
	IGenreService genreService,
	IActorService actorService,
	ICinemaHallService cinemaHallService,
	IUserService userService,
	IUserRegistrator userRegistrator,
	IContentService contentService,
	ISessionService sessionService,
	IBookingService bookingService,
	IContentGenreService contentGenreService,
	IContentActorService contentActorService) : ISeedService
{
	private readonly ILogger<SeedService> _logger = logger;
	private readonly Faker _faker = new("uk");

	private readonly IGenreService _genreService = genreService;
	private readonly IActorService _actorService = actorService;
	private readonly ICinemaHallService _cinemaHallService = cinemaHallService;
	private readonly IUserService _userService = userService;
	private readonly IUserRegistrator _userRegistrator = userRegistrator;
	private readonly IContentService _contentService = contentService;
	private readonly ISessionService _sessionService = sessionService;
	private readonly IBookingService _bookingService = bookingService;
	private readonly IContentGenreService _contentGenreService = contentGenreService;
	private readonly IContentActorService _contentActorService = contentActorService;

	private const string _placeholderBase64Image = "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";
	private int[] _ageRating = [0, 6, 12, 16, 18];
	public async Task<Result> SeedDataAsync(
		int contentCount = 20,
		int sessionsPerContent = 5,
		int genresCount = 10,
		int actorsCount = 30,
		int hallsCount = 5,
		int customersCount = 50,
		int maxBookingsPerSession = 10,
		bool uploadImages = false,
		CancellationToken cancellationToken = default)
	{
		var stopwatch = Stopwatch.StartNew();
		_logger.LogInformation("Starting database seeding (Images: {UploadImages})...", uploadImages);

		try
		{
			var genres = await GetOrCreateEntitiesAsync(genresCount, _genreService, GenerateFakeGenre, g => g.Name, cancellationToken);
			if (genres.Count == 0) return Result.Bad(Error.Failure("Seed.NoGenres", "Failed to get or create genres."));

			var actors = await GetOrCreateActorsAsync(actorsCount, uploadImages, cancellationToken);
			if (actors.Count == 0) return Result.Bad(Error.Failure("Seed.NoActors", "Failed to get or create actors."));

			var halls = await GetOrCreateEntitiesAsync(hallsCount, _cinemaHallService, GenerateFakeCinemaHall, h => h.Name, cancellationToken);
			if (halls.Count == 0) return Result.Bad(Error.Failure("Seed.NoHalls", "Failed to get or create cinema halls."));

			var customers = await GetOrCreateCustomersAsync(customersCount, cancellationToken);
			if (customers.Count == 0) return Result.Bad(Error.Failure("Seed.NoCustomers", "Failed to get or create customers."));

			var contentStopwatch = Stopwatch.StartNew();

			_logger.LogInformation("Seeding {ContentCount} Content items...", contentCount);
			List<Content> createdContents = [];
			for (int i = 0; i < contentCount; i++)
			{
				if (cancellationToken.IsCancellationRequested) return Result.Bad(Error.Failure("Seed.Cancelled", "Seeding cancelled."));

				var newContent = GenerateFakeContent();
				var contentResult = await _contentService.CreateAsync(newContent);

				if (contentResult.IsFailure)
				{
					_logger.LogWarning("[Content {Index}/{Total}] Failed to create: {Code} - {Description}", i + 1, contentCount, contentResult.Error.Code, contentResult.Error.Description);
					continue;
				}
				var createdContent = contentResult.Value!;
				createdContents.Add(createdContent);

				if (uploadImages)
				{
					var posterResult = await _contentService.SavePosterAsync(createdContent.Id, _placeholderBase64Image);
					if (posterResult.IsFailure) 
						_logger.LogWarning("[Content {ContentId}] Failed to save poster: {Error}", createdContent.Id, posterResult.Error);

					var bannerResult = await _contentService.SaveBannerAsync(createdContent.Id, _placeholderBase64Image);
					if (bannerResult.IsFailure) 
						_logger.LogWarning("[Content {ContentId}] Failed to save banner: {Error}", createdContent.Id, bannerResult.Error);
				}

				await LinkGenresAsync(createdContent, genres, cancellationToken);
				await LinkActorsAsync(createdContent, actors, cancellationToken);
				_logger.LogDebug("[Content {ContentId}] Created and linked.", createdContent.Id);
			}
			contentStopwatch.Stop();
			_logger.LogInformation("Seeded {CreatedCount} Content items in {ElapsedMilliseconds} ms.", createdContents.Count, contentStopwatch.ElapsedMilliseconds);

			var sessionBookingStopwatch = Stopwatch.StartNew();
			_logger.LogInformation("Seeding Sessions and Bookings...");
			int totalSessions = 0;
			int totalBookings = 0;

			foreach (var content in createdContents)
			{
				if (cancellationToken.IsCancellationRequested) return Result.Bad(Error.Failure("Seed.Cancelled", "Seeding cancelled."));

				int sessionCountForThisContent = _faker.Random.Int(1, sessionsPerContent);
				for (int j = 0; j < sessionCountForThisContent; j++)
				{
					var hall = _faker.PickRandom(halls);
					var newSession = GenerateFakeSession(content.Id, hall.Id);
					var sessionResult = await _sessionService.CreateAsync(newSession);

					if (sessionResult.IsFailure)
					{
						_logger.LogWarning("[Session for Content {ContentId}] Failed to create: {Error}", content.Id, sessionResult.Error);
						continue;
					}
					totalSessions++;
					var createdSession = sessionResult.Value!;

					int bookingsCreated = await CreateBookingsForSessionAsync(createdSession, hall, customers, maxBookingsPerSession, cancellationToken);
					totalBookings += bookingsCreated;
				}
			}
			sessionBookingStopwatch.Stop();
			_logger.LogInformation("Seeded {SessionCount} Sessions and {BookingCount} Bookings in {ElapsedMilliseconds} ms.", totalSessions, totalBookings, sessionBookingStopwatch.ElapsedMilliseconds);


			stopwatch.Stop();
			_logger.LogInformation("Database seeding completed successfully in {ElapsedMilliseconds} ms.", stopwatch.ElapsedMilliseconds);
			return Result.Ok();
		}
		catch (Exception ex)
		{
			stopwatch.Stop();
			_logger.LogError(ex, "An error occurred during database seeding after {ElapsedMilliseconds} ms.", stopwatch.ElapsedMilliseconds);
			return Result.Bad(Error.InternalServerError("Seed.Exception", $"Seeding failed: {ex.Message}"));
		}
	}
	private Genre GenerateFakeGenre() => new Faker<Genre>().RuleFor(g => g.Name, f => f.Music.Genre() + f.Random.Int(1, 999)).Generate();
	private Actor GenerateFakeActor() => new Faker<Actor>().RuleFor(a => a.FirstName, f => f.Name.FirstName()).RuleFor(a => a.LastName, f => f.Name.LastName()).Generate();
	private CinemaHall GenerateFakeCinemaHall() => new Faker<CinemaHall>()
				.RuleFor(h => h.Name, f => $"{f.Address.City()} {_faker.Random.Word()} Hall {f.Random.Int(1, 100)}")
				.RuleFor(h => h.SeatsPerRow, f => Enumerable.Range(0, f.Random.Int(5, 12)).Select(_ => f.Random.Int(8, 20)).ToList())
				.Generate();
	private Content GenerateFakeContent() => new Faker<Content>()
					.RuleFor(c => c.Title, f => f.Lorem.Sentence(3, 6).TrimEnd('.'))
					.RuleFor(c => c.Description, f => f.Lorem.Paragraphs(1))
					.RuleFor(c => c.Rating, f => f.Random.Bool(0.8f) ? Math.Round(f.Random.Decimal(30, 99), 1) : null)
					.RuleFor(c => c.ReleaseYear, f => f.Date.Past(20).Year)
					.RuleFor(c => c.TrailerUrl, f => f.Internet.UrlWithPath("https", "youtube.com"))
					.RuleFor(c => c.DurationMinutes, f => f.Random.Int(75, 180))
					.RuleFor(c => c.AgeRating, f => _ageRating[f.Random.Int(0, _ageRating.Length -1)]) 
					.RuleFor(c => c.DirectorFullName, f => f.Name.FullName())
					.Generate();

	private Session GenerateFakeSession(int contentId, int hallId) => new Faker<Session>()
						.RuleFor(s => s.CinemaHallId, hallId)
						.RuleFor(s => s.ContentId, contentId)
						.RuleFor(s => s.StartTime, f => f.Date.Future(refDate: DateTime.UtcNow.Date.AddDays(f.Random.Int(0, 10)).AddHours(f.Random.Int(9, 22))))
						.RuleFor(s => s.TicketPrice, f => f.Finance.Amount(50, 350, 2))
						.RuleFor(s => s.Status, SessionStatus.Scheduled)
						.Generate();

	private RegistrationUserModel GenerateFakeCustomerModel() => new Faker<RegistrationUserModel>()
				.RuleFor(u => u.FirstName, f => f.Name.FirstName())
				.RuleFor(u => u.LastName, f => f.Name.LastName())
				.RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName, f.Random.ReplaceNumbers("#####")))
				.RuleFor(u => u.Password, f => f.Internet.Password(10) + "A1a!")
				.RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("+380#########"))
				.Generate();

	private async Task<List<TEntity>> GetOrCreateEntitiesAsync<TEntity, TId>(
		int targetCount,
		IEntityService<TEntity, TId> service,
		Func<TEntity> generator,
		Func<TEntity, string> uniquePropertySelector,
		CancellationToken cancellationToken)
		where TEntity : class, IBaseEntity<TId>
		where TId : IComparable<TId>
	{
		string entityName = typeof(TEntity).Name;
		_logger.LogInformation("Ensuring {Count} {EntityName} entities exist...", targetCount, entityName);
		var existing = (await service.GetAllAsync(cancellationToken)).ToList();
		int needed = targetCount - existing.Count;

		if (needed > 0)
		{
			_logger.LogInformation("Creating {Needed} new {EntityName} entities...", needed, entityName);
			var existingUniqueProps = new HashSet<string>(existing.Select(uniquePropertySelector), StringComparer.OrdinalIgnoreCase);

			for (int i = 0; i < needed; i++)
			{
				if (cancellationToken.IsCancellationRequested)
					break;

				var newItem = generator();
				string newItemProp = uniquePropertySelector(newItem);

				if (existingUniqueProps.Contains(newItemProp))
				{
					_logger.LogDebug("Skipping duplicate {EntityName} generation for property: {PropertyValue}", entityName, newItemProp);
					i--;
					continue;
				}

				var result = await service.CreateAsync(newItem);
				if (result.IsSuccess)
				{
					existing.Add(result.Value!);
					existingUniqueProps.Add(newItemProp);
				}
				else
				{
					_logger.LogWarning("Failed to create {EntityName}: {Code} - {Description}", entityName, result.Error!.Code, result.Error.Description);
				}
			}
		}
		_logger.LogInformation("Total {EntityName} entities available: {Total}", entityName, existing.Count);
		return existing;
	}

	private async Task<List<Actor>> GetOrCreateActorsAsync(int count, bool uploadImages, CancellationToken cancellationToken)
	{
		var actors = await GetOrCreateEntitiesAsync(count, _actorService, GenerateFakeActor, a => $"{a.FirstName}-{a.LastName}", cancellationToken);

		if (uploadImages && actors.Any(a => string.IsNullOrEmpty(a.PhotoUrl)))
		{
			_logger.LogInformation("Uploading placeholder photos for Actors...");
			foreach (var actor in actors.Where(a => string.IsNullOrEmpty(a.PhotoUrl)))
			{
				if (cancellationToken.IsCancellationRequested)
					break;
				var photoResult = await _actorService.SavePhotoAsync(actor.Id, _placeholderBase64Image);

				if (photoResult.IsFailure) _logger.LogWarning("[Actor {ActorId}] Failed to save photo: {Error}", actor.Id, photoResult.Error);
			}
		}
		return actors;
	}


	private async Task<List<User>> GetOrCreateCustomersAsync(int count, CancellationToken cancellationToken)
	{
		_logger.LogInformation("Ensuring {Count} Customer Users exist...", count);

		var allUsers = await _userService.GetAllAsync(cancellationToken);

		var existingCustomers = allUsers.Where(u => u.Email != null && !u.Email.StartsWith("admin@", StringComparison.OrdinalIgnoreCase)).ToList();

		int needed = count - existingCustomers.Count;

		if (needed > 0)
		{
			_logger.LogInformation("Creating {Needed} new Customer Users...", needed);
			var existingEmails = new HashSet<string>(allUsers.Select(u => u.Email ?? ""), StringComparer.OrdinalIgnoreCase);

			for (int i = 0; i < needed; i++)
			{
				if (cancellationToken.IsCancellationRequested) break;
				var newUserModel = GenerateFakeCustomerModel();

				if (existingEmails.Contains(newUserModel.Email))
				{
					_logger.LogDebug("Skipping duplicate Customer generation for email: {Email}", newUserModel.Email);
					i--;
					continue;
				}

				var result = await _userRegistrator.RegisterCustomerAsync(newUserModel);
				if (result.IsSuccess)
				{
					existingCustomers.Add(result.Value!);
					existingEmails.Add(result.Value!.Email!);
				}
				else
				{
					_logger.LogWarning("Failed to create Customer User {Email}: {Code} - {Description}", newUserModel.Email, result.Error.Code, result.Error.Description);
				}
			}
		}
		_logger.LogInformation("Total Customer Users available: {Total}", existingCustomers.Count);
		return existingCustomers;
	}


	private async Task LinkGenresAsync(Content content, List<Genre> allGenres, CancellationToken cancellationToken)
	{
		if (allGenres.Count == 0) return;

		var genresToLink = _faker.PickRandom(allGenres, _faker.Random.Int(1, Math.Min(4, allGenres.Count))).ToList();

		foreach (var genre in genresToLink)
		{
			if (cancellationToken.IsCancellationRequested)
				break;

			var linkResult = await _contentGenreService.CreateAsync(new ContentGenre { ContentId = content.Id, GenreId = genre.Id });

			if (linkResult.IsFailure && !linkResult.Error!.Code.EndsWith("AlreadyExists"))
				_logger.LogWarning("[Content {ContentId}] Failed to link Genre {GenreId}: {Error}", content.Id, genre.Id, linkResult.Error);
		}
	}

	private async Task LinkActorsAsync(Content content, List<Actor> allActors, CancellationToken cancellationToken)
	{
		if (allActors.Count == 0) return;

		var actorsToLink = _faker.PickRandom(allActors, _faker.Random.Int(3, Math.Min(10, allActors.Count))).ToList();

		foreach (var actor in actorsToLink)
		{
			if (cancellationToken.IsCancellationRequested)
				break;

			var roleName = _faker.Name.JobTitle();
			var linkResult = await _contentActorService.CreateAsync(new ContentActor { ContentId = content.Id, ActorId = actor.Id, RoleName = roleName });

			if (linkResult.IsFailure && !linkResult.Error.Code.EndsWith("AlreadyExists"))
				_logger.LogWarning("[Content {ContentId}] Failed to link Actor {ActorId}: {Error}", content.Id, actor.Id, linkResult.Error);
		}
	}

	private async Task<int> CreateBookingsForSessionAsync(Session session, CinemaHall hall, List<User> customers, int maxBookings, CancellationToken cancellationToken)
	{
		int bookingCount = _faker.Random.Int(0, Math.Min(maxBookings, hall.TotalCapacity));
		if (bookingCount == 0 || customers.Count == 0) return 0;

		HashSet<(int, int)> bookedSeatsInSession = [];
		int createdCount = 0;

		for (int k = 0; k < bookingCount; k++)
		{
			if (cancellationToken.IsCancellationRequested) break;

			var customer = _faker.PickRandom(customers);
			(int row, int seat) targetSeat = (-1, -1);
			bool foundSeat = false;

			for (int attempt = 0; attempt < 20; attempt++)
			{
				int randomRow = _faker.Random.Int(1, hall.NumberOfRows);

				if (hall.SeatsPerRow.Count < randomRow) continue;
				int seatsInRow = hall.SeatsPerRow[randomRow - 1];

				if (seatsInRow <= 0) continue;
				int randomSeat = _faker.Random.Int(1, seatsInRow);

				if (!bookedSeatsInSession.Contains((randomRow, randomSeat)))
				{
					targetSeat = (randomRow, randomSeat);
					foundSeat = true;
					break;
				}
			}

			if (!foundSeat)
			{
				_logger.LogDebug("[Booking for Session {SessionId}] Could not find random available seat after attempts, skipping booking.", session.Id);
				continue;
			}

			var newBooking = new Booking
			{
				UserId = customer.Id,
				SessionId = session.Id,
				RowNumber = targetSeat.row,
				SeatNumber = targetSeat.seat,
				Status = _faker.PickRandomWithout(BookingStatus.Canceled)
			};

			var bookingResult = await _bookingService.CreateAsync(newBooking);

			if (bookingResult.IsSuccess)
			{
				bookedSeatsInSession.Add(targetSeat);
				createdCount++;
			}
			else if (bookingResult.Error.Code != BookingErrors.SeatIsBooked.Code)
				_logger.LogWarning("[Booking for Session {SessionId}, User {UserId}] Failed to create: {Code} - {Description}", session.Id, customer.Id, bookingResult.Error.Code, bookingResult.Error.Description);
			else
				_logger.LogTrace("[Booking for Session {SessionId}] Seat ({Row},{Seat}) conflict during seeding, ignored.", session.Id, targetSeat.row, targetSeat.seat);
		}
		return createdCount;
	}
}