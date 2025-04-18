using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class Role : IdentityRole
{
	public Role() : base() { }

	public Role(string role) : base(role) 
	{
		Id = Guid.NewGuid().ToString();
		NormalizedName = role.ToUpper();
	}
}