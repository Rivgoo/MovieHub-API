using Application.Abstractions.Repositories;
using Domain.Entities;

namespace Application.Sessions.Abstractions;

public interface ISessionRepository : IEntityOperations<Session, int>
{
}