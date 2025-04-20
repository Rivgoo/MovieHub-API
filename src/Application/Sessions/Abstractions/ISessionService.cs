using Application.Abstractions.Services;
using Domain.Entities;

namespace Application.Sessions.Abstractions;

public interface ISessionService : IEntityService<Session, int>
{
}