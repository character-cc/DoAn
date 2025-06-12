using System.Collections.ObjectModel;
using Backend.Data.Domain.Users.Enum;
using Backend.DTO.Roles;

namespace Backend.DTO.Users;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public Gender Gender { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }
    public bool Deleted { get; set; } = false;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLogin { get; set; } = null;


    public string LastLoginIp { get; set; } = string.Empty;

    public Collection<RoleDto> Roles { get; set; } = new Collection<RoleDto>();

    public string FullName
    {
        get
        {
            return $"{FirstName} {LastName}";
        }
    }
}
