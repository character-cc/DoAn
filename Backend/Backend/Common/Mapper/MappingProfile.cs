namespace Backend.Common.Mapper;

using AutoMapper;
using Backend.Common.Utils;
using Backend.Data.Domain.Users;
using Backend.DTO.Identity;
using Backend.DTO.Roles;
using Backend.DTO.Users;

public class MappingProfile : Profile
{
    public MappingProfile() 
    {
        CreateMap<RegisterRequest, User>().ForMember(dest => dest.PasswordHash ,
            opt => opt.MapFrom(src => Hasher.HashPassword(src.Password)));
        CreateMap<CreateAddressRequest, Address>();
        CreateMap<UpdateAddressRequest, Address>();
        CreateMap<Address , AddressDto>();
        CreateMap<Role, RoleDto>();
        CreateMap<User, UserDto>();
        CreateMap<CreateUserRequest, User>();
        CreateMap<UpdateUserRequest, User>();

        CreateMap<CreateRoleRequest, Role>();
        CreateMap<UpdateRoleRequest, Role>();
    }
}
