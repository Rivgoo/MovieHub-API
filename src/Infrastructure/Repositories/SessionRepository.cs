using Application.Sessions.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Abstractions;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class SessionRepository(CoreDbContext dbContext) : 
	OperationsRepository<Session, int>(dbContext), ISessionRepository
{
	public async Task<ICollection<Session>> GetAllScheduledOrOngoingWithContentAsync(CancellationToken cancellationToken = default)
	{
		return await _entities
			.AsNoTracking()
			.Include(s => s.Content)
			.Where(s => s.Status == SessionStatus.Scheduled || s.Status == SessionStatus.Ongoing)
			.ToListAsync(cancellationToken);
	}
}