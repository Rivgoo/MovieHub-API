namespace Application.Abstractions.Repositories;

/// <summary>
/// Represents a marker interface for a repository.
/// </summary>
/// <remarks>
/// This is a base interface used to identify classes that function as repositories
/// within the infrastructure layer. It does not define any members itself,
/// serving primarily for type identification and potential future extension or grouping.
/// Concrete repository interfaces and implementations will inherit from this.
/// </remarks>
public interface IRepository
{
}