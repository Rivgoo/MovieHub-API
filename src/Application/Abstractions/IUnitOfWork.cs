namespace Application.Abstractions;

/// <summary>
/// Represents the Unit of Work pattern, used to group multiple repository operations into a single transaction.
/// </summary>
/// <remarks>
/// The Unit of Work tracks changes made to entities within a business transaction
/// and coordinates the saving of those changes to the data store.
/// Implementations typically wrap a database context (like Entity Framework Core's DbContext)
/// and manage its lifecycle and the transaction scope.
/// </remarks>
public interface IUnitOfWork
{
	/// <summary>
	/// Synchronously saves all changes made in this Unit of Work to the underlying data store.
	/// </summary>
	/// <remarks>
	/// This method applies all pending insertions, updates, and deletions tracked
	/// by the Unit of Work's context within a single database transaction.
	/// It blocks the calling thread until the operation is complete.
	/// </remarks>
	void SaveChanges();

	/// <summary>
	/// Asynchronously saves all changes made in this Unit of Work to the underlying data store.
	/// </summary>
	/// <remarks>
	/// This method applies all pending insertions, updates, and deletions tracked
	/// by the Unit of Work's context within a single database transaction.
	/// It is the recommended method in asynchronous programming scenarios as it
	/// prevents blocking the calling thread.
	/// </remarks>
	/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
	/// <returns>
	/// A task that represents the asynchronous save operation.
	/// The task result contains the number of state entries written to the database.
	/// </returns>
	Task SaveChangesAsync(CancellationToken cancellationToken = default);
}