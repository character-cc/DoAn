
using System.Linq.Expressions;
using Backend.Data.Domain.Users.Enum;
using LinqToDB.Mapping;

namespace Backend.Data.Domain.Users;

[Table]
public class User : BaseEntity, ISoftDelete, IEntity
{
    [Column]
    public string Username { get; set; } = string.Empty;

    [Column]
    public string FirstName { get; set; } = string.Empty;

    [Column]
    public string LastName { get; set; } = string.Empty;
        
    [Column]
    public Gender Gender { get; set; }

    [Column]
    public string Email { get; set; } = string.Empty;

    [Column]
    public string PhoneNumber { get; set; } = string.Empty;

    [Column]
    public string PasswordHash { get; set; } = string.Empty;

    [Column, Nullable]
    public DateTime? DateOfBirth { get; set; } 

    [Column]
    public bool IsDeleted { get; set; } = false;

    [Column]
    public bool IsActive { get; set; } = true;
        
    [Column]
    public string Notes { get; set; } = string.Empty;

    [Column, Nullable]
    public DateTime? LastLogin { get; set; } = null;

    [Column]
    public string LastLoginIp { get; set; } = string.Empty;

    [Association(ThisKey = nameof(Id), OtherKey = nameof(UserRole.UserId))]
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    [Association(ExpressionPredicate = nameof(RolesExpression))]
    public ICollection<Role> Roles { get; set; }

    public static Expression<Func<User, Role, bool>> RolesExpression =>
        (user, role) => user.UserRoles.Any(ur => ur.RoleId == role.Id);

    [Association(ThisKey = nameof(Id), OtherKey = nameof(Address.UserId))]
    public ICollection<Address> Addresses { get; set; } = new List<Address>();

    [NotColumn]
    public string FullName
    {
        get { return $"{FirstName} {LastName}"; }
    }
}