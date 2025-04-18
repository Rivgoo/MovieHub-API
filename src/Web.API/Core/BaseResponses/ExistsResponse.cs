namespace Web.API.Core.BaseResponses;

/// <summary>
/// Represents a simple API response indicating whether a resource or entity exists.
/// </summary>
/// <remarks>
/// This record is used as a Data Transfer Object (DTO) for API endpoints
/// that solely need to communicate the existence status of something.
/// </remarks>
/// <param name="Exists">A value indicating whether the resource/entity exists.</param>
public record ExistsResponse(bool Exists);