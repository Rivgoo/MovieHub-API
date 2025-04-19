using Application.Users.Abstractions;
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
}