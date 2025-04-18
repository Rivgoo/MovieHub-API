namespace Application;

internal class Guard
{
	public static bool Null<T>(T? value) => value is null;

	public static bool NullOrEmpty(string? value) => string.IsNullOrEmpty(value);
	public static bool NullOrWhiteSpace(string? value) => string.IsNullOrWhiteSpace(value);
	
	public static bool MaxLength(string? value, int maxLength) => NullOrWhiteSpace(value) || value?.Length < maxLength;
	public static bool MinLength(string? value, int minLength) => NullOrWhiteSpace(value) || value?.Length > minLength;

	public static bool Min(int value, int min) => value > min;
	public static bool Max(int value, int max) => value < max;

	public static bool Min(decimal value, decimal min) => value > min;
	public static bool Max(decimal value, decimal max) => value < max;
}