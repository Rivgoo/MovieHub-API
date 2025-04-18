using Domain;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Core;

internal sealed class CoreDbContext(DbContextOptions<CoreDbContext> options) : IdentityDbContext
	<User, Role, string, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>(options)
{
	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		ApplyAuditInfo();
		return await base.SaveChangesAsync(cancellationToken);
	}
	public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
	{
		ApplyAuditInfo();
		return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
	}
	
	public override int SaveChanges()
	{
		ApplyAuditInfo();
		return base.SaveChanges();
	}
	public override int SaveChanges(bool acceptAllChangesOnSuccess)
	{
		ApplyAuditInfo();
		return base.SaveChanges(acceptAllChangesOnSuccess);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		#region Set Table Names
		modelBuilder.Entity<User>(b => b.ToTable("users"));
		modelBuilder.Entity<Role>(b => b.ToTable("roles"));
		modelBuilder.Entity<UserClaim>(b => b.ToTable("user_claims"));
		modelBuilder.Entity<UserRole>(b => b.ToTable("user_roles"));
		modelBuilder.Entity<UserLogin>(b => b.ToTable("user_logins"));
		modelBuilder.Entity<RoleClaim>(b => b.ToTable("role_claims"));
		modelBuilder.Entity<UserToken>(b => b.ToTable("user_tokens"));
		#endregion

		#region Set Identity Roles
		modelBuilder.Entity<Role>().HasData(
			new Role(RoleList.Admin),
			new Role(RoleList.Customer));
		#endregion
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