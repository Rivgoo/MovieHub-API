using Microsoft.AspNetCore.Mvc; 

namespace Web.API.Core;

/// <summary>
/// Provides a common abstract base class for all API controllers in the application.
/// </summary>
/// <remarks>
/// This class inherits from <see cref="ControllerBase"/> and is decorated with the
/// <see cref="ApiControllerAttribute"/> to enforce API-specific conventions,
/// such as attribute routing, automatic model validation errors, and binding
/// source inference.
/// Concrete API controllers should inherit from this class.
/// </remarks>
[ApiController]
public abstract class ApiController : ControllerBase
{
}