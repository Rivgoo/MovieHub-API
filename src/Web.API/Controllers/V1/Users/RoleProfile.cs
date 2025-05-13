using Application.Roles.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Web.API.Controllers.V1.Users;

/// <summary>
/// AutoMapper profile for mapping Role entities to Role DTOs.
/// </summary>
public class RoleProfile : Profile
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RoleProfile"/> class.
	/// </summary>
	public RoleProfile()
	{
		CreateMap<Role, RoleDto>();
	}
}