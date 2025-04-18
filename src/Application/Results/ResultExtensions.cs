using Application.Results;
namespace Application.Results;

/// <summary>
/// Provides extension methods for the <see cref="Result"/> and <see cref="Result{TValue}"/> classes
/// to facilitate functional-style result handling.
/// </summary>
/// <remarks>
/// These extension methods enable matching the state of a result (success or failure)
/// and executing specific logic based on that state, promoting pattern matching-like flows.
/// </remarks>
public static class ResultExtensions
{
	/// <summary>
	/// Matches the state of a <see cref="Result"/> and executes the appropriate function
	/// based on whether the result represents a success or a failure.
	/// </summary>
	/// <typeparam name="T">The return type of the success and failure functions.</typeparam>
	/// <param name="result">The <see cref="Result"/> to match. This parameter is extended.</param>
	/// <param name="onSuccess">The function to execute if the <paramref name="result"/> is a success.</param>
	/// <param name="onFailure">The function to execute if the <paramref name="result"/> is a failure.
	/// The function receives the <see cref="Error"/> from the failed result.</param>
	/// <returns>The result of executing the <paramref name="onSuccess"/> or <paramref name="onFailure"/> function.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the result is in an invalid state (e.g., IsSuccess is true but Error is not null/None).</exception>
	public static T Match<T>(
		this Result result,
		Func<T> onSuccess,
		Func<Error, T> onFailure)
	{
		if (result.IsFailure && result.Error is null)
			throw new InvalidOperationException("Result is in an invalid state: IsFailure is true but Error is null.");

		return result.IsSuccess ? onSuccess() : onFailure(result.Error!);
	}

	/// <summary>
	/// Matches the state of a <see cref="Result{TValue}"/> and executes the appropriate function
	/// based on whether the result represents a success with a value or a failure with an error.
	/// </summary>
	/// <typeparam name="T">The return type of the success and failure functions.</typeparam>
	/// <typeparam name="TValue">The type of the value contained in the result when successful.</typeparam>
	/// <param name="result">The <see cref="Result{TValue}"/> to match. This parameter is extended.</param>
	/// <param name="onSuccess">The function to execute if the <paramref name="result"/> is a success.
	/// The function receives the <see cref="Result{TValue}.Value"/> from the successful result.</param>
	/// <param name="onFailure">The function to execute if the <paramref name="result"/> is a failure.
	/// The function receives the <see cref="Error"/> from the failed result.</param>
	/// <returns>The result of executing the <paramref name="onSuccess"/> or <paramref name="onFailure"/> function.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the result is in an invalid state (e.g., IsSuccess is true but Value is null, or IsFailure is true but Error is null/None).</exception>
	public static T Match<T, TValue>(
		this Result<TValue> result,
		Func<TValue, T> onSuccess,
		Func<Error, T> onFailure)
	{
		if (result.IsFailure && result.Error is null)
			throw new InvalidOperationException("Result is in an invalid state: IsFailure is true but Error is null.");

		return result.IsSuccess ? onSuccess(result.Value!) : onFailure(result.Error!);
	}
}