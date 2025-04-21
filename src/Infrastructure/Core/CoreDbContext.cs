using Domain;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Core;

public sealed class CoreDbContext(DbContextOptions<CoreDbContext> options) : IdentityDbContext
	<User, Role, string, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>(options)
{
	#region Tables
	public DbSet<Actor> Actrors { get; set; }
	public DbSet<Booking> Bookings { get; set; }
	public DbSet<CinemaHall> CinemaHalls { get; set; }
	public DbSet<Content> Contents { get; set; }
	public DbSet<ContentActor> ContentActors { get; set; }
	public DbSet<ContentGenre> ContentGenres { get; set; }
	public DbSet<FavoriteContent> FavoriteContents { get; set; }
	public DbSet<Genre> Genres { get; set; }
	public DbSet<Session> Sessions { get; set; }
	#endregion

	#region Set up audit information
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
	#endregion

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		#region Enum Conversion
		modelBuilder.Entity<Booking>(entity =>
		{
			entity.Property(b => b.Status)
				  .HasConversion<string>()
				  .HasMaxLength(50);
		});

		modelBuilder.Entity<Session>(entity =>
		{
			entity.Property(b => b.Status)
				  .HasConversion<string>()
				  .HasMaxLength(50);
		});
		#endregion

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
			new Role("e0ddbbf0-c810-432d-8554-640db86c4443", RoleList.Admin),
			new Role("0de7a5f6-d02a-4041-9c1f-abeb8ed44c92", RoleList.Customer));
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