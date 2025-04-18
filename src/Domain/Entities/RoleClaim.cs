using Domain.Abstractions;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class RoleClaim : IdentityRoleClaim<string>, IBaseEntity<int>
{
	public RoleClaim() : base() { }

	[Required]
	public DateTime CreatedAt { get; set; }

	[Required]
	public DateTime UpdatedAt { get; set; }
}