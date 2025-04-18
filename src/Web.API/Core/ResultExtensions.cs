using Application.Results;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Core;

internal static class ResultExtensions
{
	public static IActionResult ToActionResult(this Result result)
	{
		ArgumentNullException.ThrowIfNull(result);

		if (result.IsSuccess)
			return new OkResult();
		else
			return new ObjectResult(result.Error)
			{
				StatusCode = (int)result.Error!.ToHttpStatusCode(),
				ContentTypes = { "application/json" },
			};
	}

	public static IActionResult ToActionResult<TResult>(this Result<TResult> result)
	{
		ArgumentNullException.ThrowIfNull(result);

		if (result.IsSuccess)
			if (result.Value == null)
				return new OkResult();
			else
				return new OkObjectResult(result.Value);
		else
			return new ObjectResult(result.Error)
			{
				StatusCode = (int)result.Error!.ToHttpStatusCode(),
				ContentTypes = { "application/json" },
			};
	}
	public static IActionResult ToHttpStatusResult<TResult>(this Result<TResult> result)
	{
		ArgumentNullException.ThrowIfNull(result);

		if (result.IsSuccess)
			return new OkResult();
		else
			return new ObjectResult(result.Error)
			{
				StatusCode = (int)result.Error!.ToHttpStatusCode(),
				ContentTypes = { "application/json" },
			};
	}
}