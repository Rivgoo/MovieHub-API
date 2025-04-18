using System.Text.Json.Serialization;

namespace Application.Results;

public class Result
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Result"/> class representing a successful result.
	/// </summary>
	/// <param name="isSuccess">A value indicating whether the result is successful.</param>
	protected Result(bool isSuccess = true)
	{
		IsSuccess = isSuccess;
		Error = default;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Result"/> class representing a failed result with an error.
	/// </summary>
	/// <param name="error">The error associated with the failed result.</param>
	protected Result(Error error)
	{
		IsSuccess = false;
		Error = error;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Result"/> class representing a result with an error.
	/// </summary>
	/// <param name="isSuccess">A value indicating whether the result is successful.</param>
	/// <param name="error">The error associated with the failed result.</param>
	protected Result(bool isSuccess, Error error)
	{
		IsSuccess = isSuccess;
		Error = error;
	}

	/// <summary>
	/// Gets a value indicating whether the result is a failure.
	/// </summary>
	public bool IsFailure => !IsSuccess;

	/// <summary>
	/// Gets a value indicating whether the result is successful.
	/// </summary>
	public bool IsSuccess { get; }

	/// <summary>
	/// Gets the error associated with the result, if any.
	/// </summary>
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public Error? Error { get; }

	/// <summary>
	/// Implicitly converts an <see cref="Error"/> to a <see cref="Result"/> representing a failed result.
	/// </summary>
	/// <param name="error">The error to convert.</param>
	/// <returns>A new instance of <see cref="Result"/> representing a failed result with the specified error.</returns>
	public static implicit operator Result(Error error) =>
		new(error);

	/// <summary>
	/// Creates a new instance of <see cref="Result"/> representing a successful result.
	/// </summary>
	/// <returns>A new instance of <see cref="Result"/> representing a successful result.</returns>
	public static Result Ok() =>
		new();

	/// <summary>
	/// Creates a new instance of <see cref="Result"/> representing a failed result with the specified error.
	/// </summary>
	/// <param name="error">The error associated with the failed result.</param>
	/// <returns>A new instance of <see cref="Result"/> representing a failed result with the specified error.</returns>
	public static Result Bad(Error error) =>
		new(error);

	public Result<TValue> ToValue<TValue>() => Result<TValue>.Bad(Error!);
}

public sealed class Result<TValue> : Result
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Result{TValue}"/> class with a successful result and a value.
	/// </summary>
	/// <param name="value">The value.</param>
	private Result(TValue value) : base()
		=> Value = value;

	/// <summary>
	/// Initializes a new instance of the <see cref="Result{TValue}"/> class with a failed result and an error.
	/// </summary>
	/// <param name="error">The error.</param>
	private Result(Error error) : base(error)
		=> Value = default;

	/// <summary>
	/// Gets the value of the result.
	/// </summary>
	public TValue? Value { get; private set; }

	/// <summary>
	/// Implicitly converts an <see cref="Error"/> to a <see cref="Result{TValue}"/> with a failed result.
	/// </summary>
	/// <param name="error">The error.</param>
	public static implicit operator Result<TValue>(Error error) =>
		new(error);

	/// <summary>
	/// Implicitly converts a value of type <typeparamref name="TValue"/> to a <see cref="Result{TValue}"/> with a successful result.
	/// </summary>
	/// <param name="value">The value.</param>
	public static implicit operator Result<TValue>(TValue value) =>
		new(value);

	/// <summary>
	/// Creates a new <see cref="Result{TValue}"/> with a successful result and a value.
	/// </summary>
	/// <param name="value">The value.</param>
	public static Result<TValue> Ok(TValue value) =>
		new(value);

	/// <summary>
	/// Creates a new <see cref="Result{TValue}"/> with a failed result and an error.
	/// </summary>
	/// <param name="error">The error.</param>
	public static new Result<TValue> Bad(Error error) =>
		new(error);
}