using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Application.Utilities;

/// <summary>
/// Provides utility methods for common data manipulation tasks.
/// </summary>
/// <remarks>
/// This static class contains helper methods that can be used across the application layer
/// or other layers where general utility functions are needed.
/// </remarks>
public static class StringUtilities
{
	private static readonly Regex _emailRegex = new(
		@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
		RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture,
		TimeSpan.FromSeconds(1));

	private static readonly Regex ValidPhoneNumberCharsRegex =
			new Regex(@"^[0-9\+\-\(\)\s]+$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

	/// <summary>
	/// Trims the string properties of the specified object.
	/// </summary>
	/// <remarks>
	/// This method uses reflection to find all public, readable, and writable string properties
	/// of the object and trims their values (removes leading and trailing whitespace).
	/// Null string property values remain null after trimming.
	/// </remarks>
	/// <typeparam name="T">The type of the object whose string properties are to be trimmed.</typeparam>
	/// <param name="obj">The object whose string properties are to be trimmed. Must not be null.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="obj"/> is null.</exception>
	public static void TrimStringProperties<T>(T obj)
		where T : class
	{
		ArgumentNullException.ThrowIfNull(obj);

		IEnumerable<PropertyInfo> stringProperties =
			obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(p => p.PropertyType == typeof(string) && p.CanRead && p.CanWrite);

		foreach (PropertyInfo property in stringProperties)
		{
			string? value = property.GetValue(obj) as string;

			property.SetValue(obj, SafeTrim(value));
		}
	}

	/// <summary>
	/// Checks if a given string is a valid phone number based on a basic format and digit count.
	/// </summary>
	/// <param name="phoneNumber">The phone number string to validate.</param>
	/// <param name="minimumDigitLength">The minimum number of digits required after removing non-digit characters. Defaults to 7 (a common minimum for many international numbers).</param>
	/// <param name="maximumDigitLength">The maximum number of digits allowed after removing non-digit characters. Defaults to 15 (a common maximum for many international numbers).</param>
	/// <returns><see langword="true"/> if the phone number is considered valid; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// This method performs a basic validation:
	/// 1. Checks if the string is null or empty.
	/// 2. Checks if the string contains only allowed characters (digits, space, hyphen, parentheses, plus).
	/// 3. Removes all non-digit characters and checks if the resulting string of digits meets the minimum required length.
	///
	/// This validation does NOT verify country-specific formats or the actual existence/validity of the phone number.
	/// For strict, country-specific validation, consider using a dedicated library like libphonenumber.
	/// </remarks>
	public static bool ValidatePhoneNumber(string? phoneNumber, int minimumDigitLength = 7, int maximumDigitLength = 15)
	{
		if (string.IsNullOrEmpty(phoneNumber))
			return false;

		if (!ValidPhoneNumberCharsRegex.IsMatch(phoneNumber))
			return false;

		string digitsOnly = Regex.Replace(phoneNumber, @"[^0-9]", "");

		if (digitsOnly.Length < minimumDigitLength || digitsOnly.Length > maximumDigitLength)
			return false; 

		return true;
	}

	/// <summary>
	/// Validates the format of an email address string.
	/// </summary>
	/// <remarks>
	/// Checks if the provided string follows a standard email address format.
	/// It handles null or empty strings by returning false.
	/// </remarks>
	/// <param name="email">The email address string to validate.</param>
	/// <returns><see langword="true"/> if the string is a valid email format and is not null/empty; otherwise, <see langword="false"/>.</returns>
	public static bool ValidateEmail([NotNullWhen(true)] string? email)
	{
		if (string.IsNullOrWhiteSpace(email))
			return false;

		return _emailRegex.IsMatch(email);
	}

	/// <summary>
	/// Validates if a password meets the specified identity password policy requirements.
	/// </summary>
	/// <remarks>
	/// This method manually checks the password string against the criteria defined
	/// in the provided <paramref name="passwordOptions"/>, such as minimum length,
	/// required unique characters, and inclusion of digits, lowercase, uppercase,
	/// and non-alphanumeric characters.
	/// It returns <see langword="false"/> if the password fails any of the enabled policy checks.
	/// It does NOT include checks related to user context (e.g., password containing username).
	/// </remarks>
	/// <param name="password">The password string to validate. Can be null or empty.</param>
	/// <param name="passwordOptions">The <see cref="PasswordOptions"/> defining the required password policy.</param>
	/// <returns><see langword="true"/> if the password meets all enabled policy requirements; otherwise, <see langword="false"/>.</returns>
	public static bool ValidatePassword([NotNullWhen(true)] string? password, PasswordOptions passwordOptions)
	{
		if (string.IsNullOrWhiteSpace(password))
			return false;

		if (passwordOptions.RequiredLength > 0 && password.Length < passwordOptions.RequiredLength)
			return false;

		if (passwordOptions.RequiredUniqueChars > 0 && 
			new string(password.Distinct().ToArray()).Length < passwordOptions.RequiredUniqueChars)
			return false;

		bool hasDigit = false;
		bool hasLower = false;
		bool hasUpper = false;
		bool hasNonAlphanumeric = false;

		foreach (char c in password)
		{
			if (char.IsDigit(c)) hasDigit = true;
			else if (char.IsLower(c)) hasLower = true;
			else if (char.IsUpper(c)) hasUpper = true;
			else if (!char.IsLetterOrDigit(c)) hasNonAlphanumeric = true;

			if ((!passwordOptions.RequireDigit || hasDigit) &&
				(!passwordOptions.RequireLowercase || hasLower) &&
				(!passwordOptions.RequireUppercase || hasUpper) &&
				(!passwordOptions.RequireNonAlphanumeric || hasNonAlphanumeric))
				break;
		}

		if (passwordOptions.RequireDigit && !hasDigit)
			return false;

		if (passwordOptions.RequireLowercase && !hasLower)
			return false;

		if (passwordOptions.RequireUppercase && !hasUpper)
			return false;

		if (passwordOptions.RequireNonAlphanumeric && !hasNonAlphanumeric)
			return false;

		return true;
	}

	/// <summary>
	/// Safely trims a string value, returning null if the input is null.
	/// </summary>
	/// <remarks>
	/// Unlike <see cref="string.Trim()"/> which would throw an exception on null input,
	/// this method handles null gracefully by returning null.
	/// Empty or whitespace-only strings are trimmed correctly.
	/// </remarks>
	/// <param name="value">The string value to trim.</param>
	/// <returns>The trimmed string, or <see langword="null"/> if the input value was null.</returns>
	private static string? SafeTrim(string? value) => value?.Trim();
}