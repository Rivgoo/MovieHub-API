using Domain.Entities;
using System.Text.Json.Serialization;

namespace Application.Results;

/// <summary>
/// Represents the result of an operation, indicating success or failure without a return value.
/// </summary>
/// <remarks>
/// This class follows the railway-oriented programming pattern, providing a clear
/// way to represent the outcome of operations, especially those that might fail.
/// It serves as the base class for <see cref="Result{TValue}"/>.
/// </remarks>
public class Result
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Result"/> class representing a successful result.
	/// </summary>
	/// <param name="isSuccess">A value indicating whether the result is successful. Defaults to true.</param>
	protected Result(bool isSuccess = true)
	{
		IsSuccess = isSuccess;
		Error = default;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Result"/> class representing a failed result with an error.
	/// </summary>
	/// <param name="error">The error associated with the failed result. Must not be null.</param>
	/// <exception cref="ArgumentNullException">Thrown if the provided error is null.</exception>
	protected Result(Error error) : this(false, error)
	{
		if (error is null)
			throw new ArgumentNullException(nameof(error));
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Result"/> class representing a result with a specific success status and potentially an error.
	/// </summary>
	/// <remarks>
	/// If <paramref name="isSuccess"/> is true, the <paramref name="error"/> should ideally be null or <see cref="Error.None"/>.
	/// If <paramref name="isSuccess"/> is false, the <paramref name="error"/> should represent the reason for failure.
	/// </remarks>
	/// <param name="isSuccess">A value indicating whether the result is successful.</param>
	/// <param name="error">The error associated with the result. Should be non-null if <paramref name="isSuccess"/> is false.</param>
	protected Result(bool isSuccess, Error error)
	{
		if (!isSuccess && error is null)
			throw new ArgumentNullException(nameof(error), "Error must be provided for a failed result.");

		if (isSuccess && error is not null)
			throw new ArgumentException("Error should be null or Error.None for a successful result.", nameof(error));

		IsSuccess = isSuccess;
		Error = error;
	}

	/// <summary>
	/// Gets a value indicating whether the result is a failure.
	/// </summary>
	/// <value>True if <see cref="IsSuccess"/> is false; otherwise, false.</value>
	public bool IsFailure => !IsSuccess;

	/// <summary>
	/// Gets a value indicating whether the result is successful.
	/// </summary>
	/// <value>True if the operation completed successfully; otherwise, false.</value>
	public bool IsSuccess { get; }

	/// <summary>
	/// Gets the error associated with the result, if any.
	/// </summary>
	/// <value>
	/// The <see cref="Error"/> instance if <see cref="IsFailure"/> is true; otherwise, null or <see cref="Error.None"/>.
	/// </value>
	/// <remarks>
	/// This property is ignored during JSON serialization if its value is default (null for reference types).
	/// </remarks>
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public Error? Error { get; }

	/// <summary>
	/// Implicitly converts an <see cref="Error"/> to a <see cref="Result"/> representing a failed result.
	/// </summary>
	/// <param name="error">The error to convert. Must not be null.</param>
	/// <returns>A new instance of <see cref="Result"/> representing a failed result with the specified error.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the provided error is null.</exception>
	public static implicit operator Result(Error error)
	{
		return error is null ? throw new ArgumentNullException(nameof(error)) : new(error);
	}


	/// <summary>
	/// Creates a new instance of <see cref="Result"/> representing a successful result.
	/// </summary>
	/// <returns>A new instance of <see cref="Result"/> representing a successful result.</returns>
	public static Result Ok() => new();

	/// <summary>
	/// Creates a new instance of <see cref="Result"/> representing a failed result with the specified error.
	/// </summary>
	/// <param name="error">The error associated with the failed result. Must not be null.</param>
	/// <returns>A new instance of <see cref="Result"/> representing a failed result with the specified error.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the provided error is null.</exception>
	public static Result Bad(Error error)
	{
		return error is null ? throw new ArgumentNullException(nameof(error)) : new(error);
	}


	/// <summary>
	/// Converts a failed <see cref="Result"/> to a failed <see cref="Result{TValue}"/> of a specified value type.
	/// </summary>
	/// <remarks>
	/// This method is intended for use when propagating a non-generic failure result
	/// to an operation that is expected to return a value.
	/// It should only be called on a failed result (<see cref="IsFailure"/> is true).
	/// </remarks>
	/// <typeparam name="TValue">The type of the value that the target <see cref="Result{TValue}"/> would hold on success.</typeparam>
	/// <returns>A new instance of <see cref="Result{TValue}"/> representing a failed result with the same error.</returns>
	/// <exception cref="InvalidOperationException">Thrown if the method is called on a successful result.</exception>
	public Result<TValue> ToValue<TValue>()
	{
		if (IsSuccess)
			throw new InvalidOperationException("Cannot convert a successful Result to a failed Result<TValue>.");

		if(Error is null)
			throw new InvalidOperationException("Cannot convert a Result with no error to a Result<TValue>.");

		return Result<TValue>.Bad(Error!);
	}
}

/// <summary>
/// Represents the result of an operation that returns a value upon success.
/// </summary>
/// <typeparam name="TValue">The type of the value returned by the operation on success.</typeparam>
/// <remarks>
/// This class extends <see cref="Result"/> to include a <see cref="Value"/> property
/// that holds the result data when <see cref="Result.IsSuccess"/> is true.
/// </remarks>
public sealed class Result<TValue> : Result
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Result{TValue}"/> class with a successful result and a value.
	/// </summary>
	/// <param name="value">The value to associate with the successful result.</param>
	private Result(TValue value) : base(true)
		=> Value = value;

	/// <summary>
	/// Initializes a new instance of the <see cref="Result{TValue}"/> class with a failed result and an error.
	/// </summary>
	/// <param name="error">The error to associate with the failed result. Must not be null.</param>
	/// <exception cref="ArgumentNullException">Thrown if the provided error is null.</exception>
	private Result(Error error) : base(false, error)
		=> Value = default;

	/// <summary>
	/// Gets the value of the result if the operation was successful.
	/// </summary>
	/// <value>
	/// The value of type <typeparamref name="TValue"/> if <see cref="Result.IsSuccess"/> is true;
	/// otherwise, the default value for <typeparamref name="TValue"/> (typically null for reference types and 0 or similar for value types).
	/// </value>
	public TValue? Value { get; private set; }

	/// <summary>
	/// Implicitly converts an <see cref="Error"/> to a <see cref="Result{TValue}"/> with a failed result.
	/// </summary>
	/// <param name="error">The error to convert. Must not be null.</param>
	/// <returns>A new instance of <see cref="Result{TValue}"/> representing a failed result with the specified error.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the provided error is null.</exception>
	public static implicit operator Result<TValue>(Error error)
	{
		return error is null ? throw new ArgumentNullException(nameof(error)) : new(error);
	}


	/// <summary>
	/// Implicitly converts a value of type <typeparamref name="TValue"/> to a <see cref="Result{TValue}"/> with a successful result.
	/// </summary>
	/// <param name="value">The value to convert. Can be null if <typeparamref name="TValue"/> is a reference type or nullable value type.</param>
	/// <returns>A new instance of <see cref="Result{TValue}"/> representing a successful result with the specified value.</returns>
	public static implicit operator Result<TValue>(TValue value) =>
		new(value);

	/// <summary>
	/// Creates a new <see cref="Result{TValue}"/> with a successful result and a value.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>A new instance of <see cref="Result{TValue}"/> representing a successful result with the specified value.</returns>
	public static Result<TValue> Ok(TValue value) =>
		new(value);

	/// <summary>
	/// Creates a new <see cref="Result{TValue}"/> with a failed result and an error.
	/// </summary>
	/// <param name="error">The error. Must not be null.</param>
	/// <returns>A new instance of <see cref="Result{TValue}"/> representing a failed result with the specified error.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the provided error is null.</exception>
	public static new Result<TValue> Bad(Error error)
	{
		return error is null ? throw new ArgumentNullException(nameof(error)) : new(error);
	}

	internal static Result<Content> Bad(object invalidPosterPath)
	{
		throw new NotImplementedException();
	}
}