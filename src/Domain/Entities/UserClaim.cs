using Domain.Abstractions;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class UserClaim : IdentityUserClaim<string>, IBaseEntity<int>
{
	public UserClaim() : base() { }

	[Required]
	public DateTime CreatedAt { get; set; }

	[Required]
	public DateTime UpdatedAt { get; set; }
}