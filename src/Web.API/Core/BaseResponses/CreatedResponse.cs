namespace Web.API.Core.BaseResponses;

/// <summary>
/// Represents a response for a successfully created resource, including its identifier.
/// </summary>
/// <typeparam name="TIdType">The type of the identifier for the created resource.</typeparam>
/// <remarks>
/// This record is used as a Data Transfer Object (DTO) for API endpoints
/// that return the identifier of the newly created entity upon successful creation.
/// </remarks>
/// <param name="Id">The identifier of the newly created entity.</param>
public record CreatedResponse<TIdType>(TIdType Id);