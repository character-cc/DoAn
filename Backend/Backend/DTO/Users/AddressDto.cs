using Backend.Data.Domain.Users.Enum;

namespace Backend.DTO.Users;

public class AddressDto
{

    public int Id { get; set; }

    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;
    public int ProvinceId { get; set; }
    public int DistrictId { get; set; }
    public int WardId { get; set; }
    public string StreetAddress { get; set; } = string.Empty;
    public AddressType AddressType { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsDefault { get; set; }

    public string FullAddress { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


}
