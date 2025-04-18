using System.Diagnostics.CodeAnalysis;

namespace Application;

/// <summary>
/// Provides simple guard clauses as boolean checks for common validation scenarios.
/// </summary>
/// <remarks>
/// This class offers static helper methods to perform checks like null, null or empty string,
/// and range comparisons, returning a boolean result.
/// It is designed for use in scenarios where a simple boolean check is sufficient to guard
/// against invalid conditions, rather than throwing exceptions or returning complex validation objects.
/// </remarks>
internal static class Guard
{
	/// <summary>
	/// Checks if the specified value is null.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="value">The value to check.</param>
	/// <returns><see langword="true"/> if the value is null; otherwise, <see langword="false"/>.</returns>
	public static bool Null<T>([NotNullWhen(false)] T? value) => value is null;

	/// <summary>
	/// Checks if the specified string value is null or an empty string ("").
	/// </summary>
	/// <param name="value">The string value to check.</param>
	/// <returns><see langword="true"/> if the string is null or empty; otherwise, <see langword="false"/>.</returns>
	public static bool NullOrEmpty([NotNullWhen(false)] string? value) => string.IsNullOrEmpty(value);

	/// <summary>
	/// Checks if the specified string value is null, empty (""), or consists only of white-space characters.
	/// </summary>
	/// <param name="value">The string value to check.</param>
	/// <returns><see langword="true"/> if the string is null, empty, or white-space; otherwise, <see langword="false"/>.</returns>
	public static bool NullOrWhiteSpace([NotNullWhen(false)] string? value) => string.IsNullOrWhiteSpace(value);

	/// <summary>
	/// Checks if the specified string value is strictly longer than the maximum allowed length.
	/// </summary>
	/// <param name="value">The string value to check.</param>
	/// <param name="maxLength">The maximum allowed length.</param>
	/// <returns><see langword="true"/> if the string is not null/white-space and its length is greater than <paramref name="maxLength"/>; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// This method returns <see langword="true"/> if the <paramref name="value"/> is not null or white-space, and its length is strictly greater than <paramref name="maxLength"/> (<c>value.Length > maxLength</c>).
	/// Null, empty, or white-space strings are NOT considered too long by this check.
	/// </remarks>
	public static bool MaxLength(string? value, int maxLength) => NullOrWhiteSpace(value) && value!.Length > maxLength;


	/// <summary>
	/// Checks if the specified string value is null, empty, white-space, or its length is strictly less than the minimum required length.
	/// </summary>
	/// <param name="value">The string value to check.</param>
	/// <param name="minLength">The minimum required length.</param>
	/// <returns><see langword="true"/> if the string is null, empty, white-space, or too short; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// This method returns <see langword="true"/> if <see cref="NullOrWhiteSpace(string?)"/> is true,
	/// or if the non-null/non-whitespace <paramref name="value"/> has a length strictly less than <paramref name="minLength"/> (<c>value.Length < minLength</c>).
	/// Null, empty, and white-space strings are considered too short by this check if <paramref name="minLength"/> > 0.
	/// </remarks>
	public static bool MinLength([NotNullWhen(false)] string? value, int minLength) => NullOrWhiteSpace(value) || value!.Length < minLength;


	/// <summary>
	/// Checks if the specified integer value is strictly less than the minimum allowed value.
	/// </summary>
	/// <param name="value">The integer value to check.</param>
	/// <param name="min">The minimum allowed value.</param>
	/// <returns><see langword="true"/> if the value is less than the minimum; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// This method returns <see langword="true"/> if <paramref name="value"/> < <paramref name="min"/>.
	/// </remarks>
	public static bool Min(int value, int min) => value < min;

	/// <summary>
	/// Checks if the specified integer value is strictly greater than the maximum allowed value.
	/// </summary>
	/// <param name="value">The integer value to check.</param>
	/// <param name="max">The maximum allowed value.</param>
	/// <returns><see langword="true"/> if the value is greater than the maximum; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// This method returns <see langword="true"/> if <paramref name="value"/> > <paramref name="max"/>.
	/// </remarks>
	public static bool Max(int value, int max) => value > max;

	/// <summary>
	/// Checks if the specified decimal value is strictly less than the minimum allowed value.
	/// </summary>
	/// <param name="value">The decimal value to check.</param>
	/// <param name="min">The minimum allowed value.</param>
	/// <returns><see langword="true"/> if the value is less than the minimum; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// This method returns <see langword="true"/> if <paramref name="value"/> < <paramref name="min"/>.
	/// </remarks>
	public static bool Min(decimal value, decimal min) => value < min;

	/// <summary>
	/// Checks if the specified decimal value is strictly greater than the maximum allowed value.
	/// </summary>
	/// <param name="value">The decimal value to check.</param>
	/// <param name="max">The maximum allowed value.</param>
	/// <returns><see langword="true"/> if the value is greater than the maximum; otherwise, <see langword="false"/>.</returns>
	/// <remarks>
	/// This method returns <see langword="true"/> if <paramref name="value"/> > <paramref name="max"/>.
	/// </remarks>
	public static bool Max(decimal value, decimal max) => value > max;
}