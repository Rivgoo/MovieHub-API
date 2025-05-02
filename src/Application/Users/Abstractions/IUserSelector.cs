using Application.Filters.Abstractions;
using Application.Users.Dtos;
using Domain.Entities;

namespace Application.Users.Abstractions;

/// <summary>
/// Defines the contract for a selector that projects User entities to UserDto objects.
/// </summary>
public interface IUserSelector : ISelector<User, UserDto>
{
}