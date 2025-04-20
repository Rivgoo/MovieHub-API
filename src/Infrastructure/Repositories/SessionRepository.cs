using Application.Sessions.Abstractions;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Core;

namespace Infrastructure.Repositories;

internal class SessionRepository(CoreDbContext dbContext) : 
	OperationsRepository<Session, int>(dbContext), ISessionRepository
{
}