using Application.Abstractions.Repositories;
using Application.Users.Models;
using Domain.Entities;

namespace Application.Users.Abstractions;

public interface IUserRepository : IEntityOperations<User, string>
{
	Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task<UserInfo?> GetUserInfoByIdAsync(string id, CancellationToken cancellationToken);
	Task UpdateLastLoginDateAsync(string id, DateTime utcNow);
}