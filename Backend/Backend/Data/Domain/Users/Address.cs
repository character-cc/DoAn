using Backend.Data.Domain.Users.Enum;
using LinqToDB.Mapping;

namespace Backend.Data.Domain.Users;

[Table]
public class Address : BaseEntity, IEntity
{
    [Column]
    public string Title { get; set; } = string.Empty;

    [Column]
    public string RecipientName { get; set; } = string.Empty;

    [Column]
    public string RecipientPhone { get; set; } = string.Empty;

    [Column]
    public int ProvinceId { get; set; }

    [Column]
    public int DistrictId { get; set; }

    [Column]
    public int WardId { get; set; }

    [Column]
    public string StreetAddress { get; set; } = string.Empty;

    [Column]
    public AddressType AddressType { get; set; }

    [Column, Nullable]
    public string Notes { get; set; }

    [Column]
    public bool IsDefault { get; set; }

    [Column]
    public int UserId { get; set; }

    [Association(ThisKey = nameof(UserId), OtherKey = nameof(User.Id))]
    public User User { get; set; }

    [Column]
    public string FullAddress { get; set; }
}

