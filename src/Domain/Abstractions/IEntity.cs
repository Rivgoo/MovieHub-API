namespace Domain.Abstractions;

/// <summary>
/// Represents a core concept of a domain entity.
/// </summary>
/// <remarks>
/// This is a marker interface used to identify classes that are considered entities
/// within the domain model. It does not contain any properties or methods itself,
/// but can be used for generic constraints and type checking to enforce domain design principles.
/// </remarks>
public interface IEntity { }