using Application.Abstractions.Repositories;
using Domain.Entities;

namespace Application.Users.Abstractions;

public interface IUserRepository : IEntityOperations<User, string>
{
	Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}