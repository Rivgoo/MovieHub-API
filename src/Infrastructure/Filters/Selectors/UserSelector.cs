using Application.Users.Abstractions;
using Application.Users.Dtos;
using Domain.Entities;
using Infrastructure.Core;
using Infrastructure.Filters.Abstractions;

namespace Infrastructure.Filters.Selectors;

internal class UserSelector(CoreDbContext dbContext) :
	BaseSelector<User, UserDto>(dbContext), IUserSelector
{
	public override IQueryable<UserDto> Select(IQueryable<User> source)
	{
		return source.Select(u => new UserDto
		{
			Id = u.Id,
			FirstName = u.FirstName,
			LastName = u.LastName,
			Email = u.Email ?? string.Empty,
			UserName = u.UserName ?? string.Empty,
			PhoneNumber = u.PhoneNumber,
			EmailConfirmed = u.EmailConfirmed,
			IsBlocked = u.IsBlocked,
			CreatedAt = u.CreatedAt,
			UpdatedAt = u.UpdatedAt,
			LastLoginAt = u.LastLoginAt,
			Roles = _dbContext.UserRoles.Where(ur => ur.UserId == u.Id).Select(r => r.RoleId).ToList()
		});
	}
}