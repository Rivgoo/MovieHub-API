using Domain.Abstractions;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class UserLogin : IdentityUserLogin<string>, IAuditableEntity
{
	public UserLogin() : base() { }

	[Required]
	public DateTime CreatedAt { get; set; }
	[Required]
	public DateTime UpdatedAt { get; set; }
}