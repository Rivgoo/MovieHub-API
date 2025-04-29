using Application.Results;
using Application.Seeds.Abstractions;
using Asp.Versioning;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.API.Core;

namespace Web.API.Controllers.V1.Utils;

/// <summary>
/// Controller for utility operations like database seeding.
/// </summary>
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/utils/seed")]
[ApiController]
public class SeedController : ApiController
{
	private readonly ISeedService _seedService;
	private readonly IConfiguration _configuration;
	private readonly IHostEnvironment _environment;
	private readonly ILogger<SeedController> _logger;

	public SeedController(
		ISeedService seedService,
		IConfiguration configuration,
		IHostEnvironment environment,
		ILogger<SeedController> logger)
	{
		_seedService = seedService;
		_configuration = configuration;
		_environment = environment;
		_logger = logger;
	}

	/// <summary>
	/// Seeds the database with fake data. Restricted to Admins and Development environment or explicit config enable.
	/// </summary>
	/// <param name="contentCount">Number of Content items to create.</param>
	/// <param name="sessionsPerContent">Maximum number of Sessions per Content.</param>
	/// <param name="genresCount">Number of Genres to ensure exist.</param>
	/// <param name="actorsCount">Number of Actors to ensure exist.</param>
	/// <param name="hallsCount">Number of CinemaHalls to ensure exist.</param>
	/// <param name="customersCount">Number of Customer Users to ensure exist.</param>
	/// <param name="maxBookingsPerSession">Maximum number of Bookings per Session.</param>
	/// <param name="uploadImages">Upload placeholder images for Content/Actors.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <response code="200">Indicates successful seeding.</response>
	/// <response code="400">If seeding fails validation or is disabled.</response>
	/// <response code="403">If the user is not an Admin or not in allowed environment/config.</response>
	/// <response code="500">If an internal error occurs during seeding.</response>
	[HttpPost]
	[Authorize(Roles = RoleList.Admin)]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> SeedDatabase(
		[FromQuery] int contentCount = 20,
		[FromQuery] int sessionsPerContent = 5,
		[FromQuery] int genresCount = 10,
		[FromQuery] int actorsCount = 30,
		[FromQuery] int hallsCount = 5,
		[FromQuery] int customersCount = 50,
		[FromQuery] int maxBookingsPerSession = 10,
		[FromQuery] bool uploadImages = false,
		CancellationToken cancellationToken = default)
	{
		bool seedEnabledByConfig = _configuration.GetValue<bool>("FakeDataSeed:Enabled", false);

		if (!seedEnabledByConfig)
		{
			_logger.LogWarning("Database seeding is disabled via 'FakeDataSeed:Enabled=false'.");
			return Result.Bad(Error.BadRequest("Seed.Disabled", "Database seeding is disabled via configuration.")).ToActionResult();
		}

		_logger.LogInformation("Initiating database seed via API endpoint by User {User}", User.Identity?.Name ?? "Unknown");

		contentCount = Math.Clamp(contentCount, 0, 500);
		sessionsPerContent = Math.Clamp(sessionsPerContent, 0, 20);
		genresCount = Math.Clamp(genresCount, 0, 50);
		actorsCount = Math.Clamp(actorsCount, 0, 200);
		hallsCount = Math.Clamp(hallsCount, 0, 20);
		customersCount = Math.Clamp(customersCount, 0, 500);
		maxBookingsPerSession = Math.Clamp(maxBookingsPerSession, 0, 50);

		var result = await _seedService.SeedDataAsync(
			contentCount,
			sessionsPerContent,
			genresCount,
			actorsCount,
			hallsCount,
			customersCount,
			maxBookingsPerSession,
			uploadImages,
			cancellationToken);

		return result.ToActionResult();
	}
}