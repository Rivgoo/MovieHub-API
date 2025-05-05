using Application.Filters.Abstractions;
using Application.Sessions.Dtos;
using Domain.Entities;

namespace Application.Sessions.Abstractions;

public interface ISessionContentSelector : ISelector<Session, SessionContentDto>
{
}