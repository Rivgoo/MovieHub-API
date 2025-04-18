using Application.Abstractions;
using Application.Abstractions.Services;
using Application.Results;
using Application.Users.Abstractions;
using Application.Users.Models;
using Application.Utilities;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Application.Users;

internal class UserService(
	UserManager<User> userManager,
	SignInManager<User> signInManager,
	IUserRepository entityRepository,
	IOptions<IdentityOptions> identityOptionsAccessor,
	IUnitOfWork unitOfWork) : 
	BaseEntityService<User, string, IUserRepository>(entityRepository, unitOfWork), IUserService
{
	private readonly UserManager<User> _userManager = userManager;
	private readonly SignInManager<User> _signInManager = signInManager;
	private readonly PasswordOptions _passwordOptions = identityOptionsAccessor.Value.Password;

	public async Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		var user = await entityRepository.GetByEmailAsync(email, cancellationToken);

		if (user == null)
			return Result<User>.Bad(EntityErrors<User, string>.NotFound(email));

		return Result<User>.Ok(user);
	}

	public async Task<AuthenticationResult> TryAuthentication(string email, string password)
	{
		var result = new AuthenticationResult();

		if(StringUtilities.ValidateEmail(email) == false)
		{
			result.IsInvalidInput = true;
			return result;
		}

		var user = await entityRepository.GetByEmailAsync(email);

		if(user == null)
		{
			result.IsInvalidInput = true;
			return result;
		}

		if(user.IsBlocked)
		{
			result.IsBlocked = true;
			return result;
		}

		if(!user.EmailConfirmed)
		{
			result.IsEmailNotConfirmed = true;
			return result;
		}

		var signInResult = await _signInManager.PasswordSignInAsync(email, password, false, true);

		if (signInResult.Succeeded)
		{
			var role = (await _userManager.GetRolesAsync(user))[0];
			result.User = new(user.Id, role);
			result.Succeeded = true;

			return result;
		}

		if(signInResult.IsLockedOut)
		{
			result.IsLockedOut = true;
			return result;
		}

		result.IsInvalidInput = true;

		return result;
	}

	protected override async Task<Result> ValidateEntityAsync(User entity)
	{
		StringUtilities.TrimStringProperties(entity);

		if (Guard.MinLength(entity.FirstName, 1))
			return Result.Bad(EntityErrors<User, string>.StringTooShort(nameof(entity.FirstName), 1));

		if (Guard.MaxLength(entity.FirstName, 255))
			return Result.Bad(EntityErrors<User, string>.StringTooLong(nameof(entity.FirstName), 255));

		if (Guard.MinLength(entity.LastName, 1))
			return Result.Bad(EntityErrors<User, string>.StringTooShort(nameof(entity.LastName), 1));

		if (Guard.MaxLength(entity.LastName, 255))
			return Result.Bad(EntityErrors<User, string>.StringTooLong(nameof(entity.LastName), 255));

		return Result.Ok();
	}
}