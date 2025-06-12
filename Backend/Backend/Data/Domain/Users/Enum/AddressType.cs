using System.ComponentModel.DataAnnotations;

namespace Backend.Data.Domain.Users.Enum;

public enum AddressType
{
    [Display(Name = "Nhà Riêng")]
    Home = 1,

    [Display(Name = "Cơ Quan")]
    Office = 2,

    [Display(Name = "Khác")]
    Other = 3
}
