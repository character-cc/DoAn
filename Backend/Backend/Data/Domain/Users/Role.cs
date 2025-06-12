using LinqToDB.Mapping;

namespace Backend.Data.Domain.Users;

[Table]
public class Role : BaseEntity, IEntity
{
    [Column]
    public string FriendlyName { get; set; } = string.Empty;

    [Column]
    public string SystemName { get; set; } = string.Empty;

    [Column]
    public bool IsActive { get; set; } = true;

    [Column]
    public bool IsFreeShipping { get; set; } = false;

    [Column]
    public bool IsSystemRole { get; set; } = false;

    [Association(ThisKey = nameof(Id), OtherKey = nameof(UserRole.RoleId))]
    public ICollection<User> Users { get; set; } = new List<User>();
}