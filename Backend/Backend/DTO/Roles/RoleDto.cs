namespace Backend.DTO.Roles;

public class RoleDto
{
    public int Id { get; set; }
    public string FriendlyName { get; set; }

    public string SystemName { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsFreeShipping { get; set; } = false;

    public bool IsSystemRole { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
