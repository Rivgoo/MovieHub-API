using Application.Users.Abstractions;
using Application.Users.Dtos;
using Application.Users.Models;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal class UserRepository(CoreDbContext dbContext) :
	OperationsRepository<User, string>(dbContext), IUserRepository
{
	public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		return await _entities
			.AnyAsync(x => x.Email!.Equals(email), cancellationToken);
	}

	public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		return await _entities.AsNoTracking()
			.Where(x => x.Email!.Equals(email))
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<UserDto?> GetDtoByIdAsync(string id, CancellationToken cancellationToken)
	{
		return await _entities.AsNoTracking()
			.Where(u => u.Id == id)
			.Select(u => new UserDto
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
			})
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<UserInfo?> GetUserInfoByIdAsync(string id, CancellationToken cancellationToken)
	{
		return await _entities.AsNoTracking()
			.Where(x => x.Id!.Equals(id))
			.Select(x => new UserInfo
			{
				FirstName = x.FirstName,
				LastName = x.LastName,
				Email = x.Email!
			})
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task UpdateLastLoginDateAsync(string id, DateTime utcNow)
	{
		await _entities
			.Where(x => x.Id!.Equals(id))
			.ExecuteUpdateAsync(x => x
				.SetProperty(e => e.LastLoginAt, utcNow));
	}
}