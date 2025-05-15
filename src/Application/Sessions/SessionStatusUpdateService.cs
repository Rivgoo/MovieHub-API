using Application.Abstractions;
using Application.Sessions.Abstractions;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Sessions;

internal class SessionStatusUpdateService : BackgroundService
{
	private readonly ILogger<SessionStatusUpdateService> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly TimeSpan _checkInterval;

	public SessionStatusUpdateService(
		ILogger<SessionStatusUpdateService> logger,
		IServiceProvider serviceProvider,
		IConfiguration configuration)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
		_checkInterval = TimeSpan.FromMinutes(configuration.GetValue("SessionStatusUpdate:CheckIntervalMinutes", 1));

		_logger.LogInformation("SessionStatusUpdateService configured with CheckInterval: {CheckInterval}", _checkInterval);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("SessionStatusUpdateService is starting.");
		stoppingToken.Register(() => _logger.LogInformation("SessionStatusUpdateService is stopping."));

		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				using var scope = _serviceProvider.CreateScope();
				var sessionRepository = scope.ServiceProvider.GetRequiredService<ISessionRepository>();
				var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

				var sessionsToProcess = await sessionRepository.GetAllScheduledOrOngoingWithContentAsync(stoppingToken);

				if (sessionsToProcess.Count != 0)
				{
					int updatedCount = 0;
					DateTime utcNow = DateTime.UtcNow;

					foreach (var session in sessionsToProcess)
					{
						if (stoppingToken.IsCancellationRequested)
						{
							_logger.LogInformation("Cancellation requested during session status processing.");
							break;
						}

						SessionStatus originalStatus = session.Status;
						DateTime sessionEndTime = session.StartTime.AddMinutes(session.Content.DurationMinutes);

						if (session.Status == SessionStatus.Scheduled)
						{ 	
							if (utcNow >= session.StartTime && utcNow < sessionEndTime)
								session.Status = SessionStatus.Ongoing;
							else if (utcNow >= sessionEndTime)
								session.Status = SessionStatus.Ended;
						}
						else if (session.Status == SessionStatus.Ongoing)
							if (utcNow >= sessionEndTime)
								session.Status = SessionStatus.Ended;

						if (session.Status != originalStatus)
						{
							sessionRepository.Update(session);
							updatedCount++;
						}
					}

					if (updatedCount > 0)
						await unitOfWork.SaveChangesAsync(stoppingToken);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred while updating session statuses.");
			}

			try
			{
				await Task.Delay(_checkInterval, stoppingToken);
			}
			catch (OperationCanceledException)
			{
				_logger.LogInformation("SessionStatusUpdateService delay was canceled.");
			}
		}

		_logger.LogInformation("SessionStatusUpdateService has stopped.");
	}
}