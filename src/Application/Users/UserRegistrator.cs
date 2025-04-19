using Application.Results;
using Application.Users.Abstractions;
using Application.Users.Models;
using Application.Utilities;
using Domain;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Application.Users;

internal class UserRegistrator(
	IOptions<IdentityOptions> identityOptionsAccessor,
	UserManager<User> userManager,
	IUserService userService) : IUserRegistrator
{
	private readonly IUserService _userService = userService;
	private readonly UserManager<User> _userManager = userManager;
	private readonly PasswordOptions _passwordOptions = identityOptionsAccessor.Value.Password;

	public async Task<Result<User>> RegisterAdminAsync(RegistrationUserModel model)
	{
		return await RegisterUserAsync(model, RoleList.Admin, true);
	}
	public async Task<Result<User>> RegisterCustomerAsync(RegistrationUserModel model)
	{
		if (StringUtilities.ValidatePhoneNumber(model.PhoneNumber) == false)
			return Result<User>.Bad(UserErrors.InvalidPhoneNumber);

		return await RegisterUserAsync(model, RoleList.Customer, true);
	}

	private async Task<Result<User>> RegisterUserAsync(
		RegistrationUserModel model, string role, bool isEmailConfirmed)
	{
		var user = new User
		{
			Email = model.Email,
			NormalizedEmail = model.Email.ToUpperInvariant(),
			UserName = model.Email,
			NormalizedUserName = model.Email.ToUpperInvariant(),
			FirstName = model.FirstName,
			LastName = model.LastName,
			PhoneNumber = model.PhoneNumber,
			PhoneNumberConfirmed = true,
			IsBlocked = false,
			EmailConfirmed = isEmailConfirmed,
			LockoutEnabled = true,
		};

		if (StringUtilities.ValidatePassword(model.Password, _passwordOptions) == false)
			return Result<User>.Bad(UserErrors.InvalidPassword);

		var result = await _userService.CreateAsync(user);

		if (result.IsSuccess)
		{
			await _userManager.AddToRoleAsync(user, role);
			await _userManager.AddPasswordAsync(user, model.Password);

			return Result<User>.Ok(user);
		}

		return result;
	}
}