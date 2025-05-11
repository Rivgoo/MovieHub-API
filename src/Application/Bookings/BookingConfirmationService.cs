using Application.Bookings.Abstractions;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Application.Abstractions;

namespace Application.Bookings;

internal class BookingConfirmationService : BackgroundService
{
	private readonly ILogger<BookingConfirmationService> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly IConfiguration _configuration;

	private readonly TimeSpan _checkInterval;
	private readonly TimeSpan _confirmationAgeThreshold;

	public BookingConfirmationService(
		ILogger<BookingConfirmationService> logger,
		IServiceProvider serviceProvider,
		IConfiguration configuration)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
		_configuration = configuration;

		_checkInterval = TimeSpan.FromMinutes(_configuration.GetValue("BookingConfirmation:CheckIntervalMinutes", 3));
		_confirmationAgeThreshold = TimeSpan.FromMinutes(_configuration.GetValue("BookingConfirmation:ConfirmationAgeMinutes", 3));

		_logger.LogInformation("BookingConfirmationService configured with CheckInterval: {CheckInterval} and ConfirmationAgeThreshold: {ConfirmationAgeThreshold}",
			_checkInterval, _confirmationAgeThreshold);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("BookingConfirmationService is starting.");

		stoppingToken.Register(() => _logger.LogInformation("BookingConfirmationService is stopping."));

		while (!stoppingToken.IsCancellationRequested)
		{
			_logger.LogInformation("BookingConfirmationService is working. Checking for pending bookings to confirm.");

			try
			{
				using var scope = _serviceProvider.CreateScope();
				var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
				var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

				var pendingBookings = await bookingRepository.GetAllWithPendingStatusAsync(stoppingToken);

				if (pendingBookings.Count == 0)
					_logger.LogInformation("No pending bookings found.");
				else
				{
					_logger.LogInformation("Found {Count} pending bookings. Processing...", pendingBookings.Count);

					DateTime utcNow = DateTime.UtcNow;
					int confirmedCount = 0;

					foreach (var booking in pendingBookings)
					{
						if (stoppingToken.IsCancellationRequested)
						{
							_logger.LogInformation("Cancellation requested during booking processing.");
							break;
						}

						if (utcNow - booking.CreatedAt >= _confirmationAgeThreshold)
						{
							booking.Status = BookingStatus.Confirmed;
							bookingRepository.Update(booking);
						}
					}

					await unitOfWork.SaveChangesAsync();

					_logger.LogInformation("Processed pending bookings. {ConfirmedCount} bookings were confirmed.", confirmedCount);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while checking and confirming bookings.");
			}

			try
			{
				await Task.Delay(_checkInterval, stoppingToken);
			}
			catch (OperationCanceledException)
			{
				_logger.LogInformation("BookingConfirmationService delay was canceled.");
			}
		}

		_logger.LogInformation("BookingConfirmationService has stopped.");
	}
}