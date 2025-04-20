using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class Role : IdentityRole
{
	public Role() : base() { }

	public Role(string role) : base(role)
	{
		NormalizedName = role.ToUpper();
	}

	public Role(string id, string role) : base(role)
	{
		Id = id;
		NormalizedName = role.ToUpper();
	}
}