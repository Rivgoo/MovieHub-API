using Application.Abstractions.Services;
using Application.Results;
using Application.Users.Models;
using Domain.Entities;

namespace Application.Users.Abstractions;

public interface IUserService : IEntityService<User, string>
{
	Task<Result<UserInfo>> GetUserInfoByIdAsync(string id, CancellationToken cancellationToken = default);
	Task<Result<bool>> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task<AuthenticationResult> TryAuthentication(string email, string password);
}