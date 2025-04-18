using Domain.Abstractions;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class UserToken : IdentityUserToken<string>, IAuditableEntity
{
	[Required]
	public DateTime CreatedAt { get; set; }

	[Required]
	public DateTime UpdatedAt { get; set; }
}