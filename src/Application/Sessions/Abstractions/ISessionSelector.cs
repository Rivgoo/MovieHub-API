using Application.Filters.Abstractions;
using Application.Sessions.Models;
using Domain.Entities;

namespace Application.Sessions.Abstractions;

public interface ISessionSelector : ISelector<Session, SessionDto>
{
}