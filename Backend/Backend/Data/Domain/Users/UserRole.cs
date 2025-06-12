using LinqToDB.Mapping;

namespace Backend.Data.Domain.Users;

[Table]
public class UserRole : IEntity
{
    [Column, PrimaryKey]
    public int UserId { get; set; }

    [Column, PrimaryKey]
    public int RoleId { get; set; }

    [Association(ThisKey = nameof(UserId), OtherKey = nameof(User.Id))]
    public User User { get; set; }

    [Association(ThisKey = nameof(RoleId), OtherKey = nameof(Role.Id))]
    public Role Role { get; set; }
}