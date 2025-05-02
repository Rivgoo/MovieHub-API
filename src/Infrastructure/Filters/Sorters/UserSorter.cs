using Application.Filters.Abstractions;
using Application.Users;
using Domain.Entities;
using Infrastructure.Core;
using Infrastructure.Filters.Abstractions;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Filters.Sorters;

internal class UserSorter(CoreDbContext dbContext)
	: BaseSorter<User, UserFilter>(dbContext), ISorter<User, UserFilter>
{
	public override IQueryable<User> GetSort(UserFilter filter)
	{
		var query = PredicateBuilder.New<User>(true);

		var firstNameTerm = filter.FirstName?.Trim();
		if (!string.IsNullOrEmpty(firstNameTerm))
			query = query.And(u => EF.Functions.Like(u.FirstName, $"%{firstNameTerm}%"));

		var lastNameTerm = filter.LastName?.Trim();
		if (!string.IsNullOrEmpty(lastNameTerm))
			query = query.And(u => EF.Functions.Like(u.LastName, $"%{lastNameTerm}%"));

		var emailTerm = filter.Email?.Trim();
		if (!string.IsNullOrEmpty(emailTerm))
			query = query.And(u => EF.Functions.Like(u.Email!, $"%{emailTerm}%"));

		var userNameTerm = filter.UserName?.Trim();
		if (!string.IsNullOrEmpty(userNameTerm))
			query = query.And(u => EF.Functions.Like(u.UserName!, $"%{userNameTerm}%"));

		var phoneTerm = filter.PhoneNumber?.Trim();
		if (!string.IsNullOrEmpty(phoneTerm))
			query = query.And(u => u.PhoneNumber != null && EF.Functions.Like(u.PhoneNumber, $"%{phoneTerm}%"));

		if (filter.IsBlocked.HasValue)
			query = query.And(u => u.IsBlocked == filter.IsBlocked.Value);

		if (filter.EmailConfirmed.HasValue)
			query = query.And(u => u.EmailConfirmed == filter.EmailConfirmed.Value);

		if (filter.MinCreatedAt.HasValue)
			query = query.And(u => u.CreatedAt >= filter.MinCreatedAt.Value);

		if (filter.MaxCreatedAt.HasValue)
			query = query.And(u => u.CreatedAt < filter.MaxCreatedAt.Value.AddDays(1));

		IQueryable<User> baseQuery = _entities.AsExpandable();

		if (!string.IsNullOrWhiteSpace(filter.RoleId))
		{
			var normalizedRoleName = filter.RoleId.ToUpperInvariant();
			query = query.And(u => _dbContext.UserRoles
									.Where(ur => ur.UserId == u.Id)
									.Any(x => x.RoleId == filter.RoleId));
		}

		return baseQuery.Where(query);
	}
}