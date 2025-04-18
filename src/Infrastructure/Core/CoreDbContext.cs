using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Core;

internal sealed class CoreDbContext(DbContextOptions<CoreDbContext> options) : DbContext(options)
{
	public override int SaveChanges(bool acceptAllChangesOnSuccess)
	{
		ApplyAuditInfo();
		return base.SaveChanges(acceptAllChangesOnSuccess);
	}
	public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
	{
		ApplyAuditInfo();
		return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
	}
	private void ApplyAuditInfo()
	{
		var entries = ChangeTracker
			.Entries()
			.Where(e => e.Entity is IAuditableEntity &&
						(e.State == EntityState.Added || e.State == EntityState.Modified));

		var now = DateTime.UtcNow;

		foreach (var entry in entries)
		{
			var entity = (IAuditableEntity)entry.Entity;

			if (entry.State == EntityState.Added)
			{
				entity.CreatedAt = now;
				entity.UpdatedAt = now;
			}
			else if (entry.State == EntityState.Modified)
			{
				entity.UpdatedAt = now;
				entry.Property(nameof(IAuditableEntity.CreatedAt)).IsModified = false;
			}
		}
	}
}