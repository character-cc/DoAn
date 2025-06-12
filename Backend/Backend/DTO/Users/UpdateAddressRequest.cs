using Backend.Data.Domain.Users.Enum;
using FluentValidation;

namespace Backend.DTO.Users;

public class UpdateAddressRequest
{
    public int UserId { get; set; }

    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    public string RecipientPhone { get; set; } = string.Empty;

    public int ProvinceId { get; set; }
    public int DistrictId { get; set; }
    public int WardId { get; set; }
    public string StreetAddress { get; set; } = string.Empty;
    public AddressType AddressType { get; set; }

    public string FullAddress { get; set; }
    public string Notes { get; set; }
    public bool IsDefault { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class UpdateAddressRequestValidator : AbstractValidator<UpdateAddressRequest>
{
    public UpdateAddressRequestValidator()
    {

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID phải lớn hơn 0");
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Address ID must be greater than 0.");
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tên của địa chỉ được yêu cầu")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
        RuleFor(x => x.RecipientName)
            .NotEmpty().WithMessage("Recipient name is required.")
            .MaximumLength(100).WithMessage("Recipient name cannot exceed 100 characters.");
        RuleFor(x => x.RecipientPhone)
            .NotEmpty().WithMessage("Recipient phone is required.")
            .Matches(@"^\+?[0-9]{9,15}$").WithMessage("Phone number format is invalid.");
        RuleFor(x => x.ProvinceId)
            .GreaterThan(0).WithMessage("Province ID must be greater than 0.");
        RuleFor(x => x.DistrictId)
            .GreaterThan(0).WithMessage("District ID must be greater than 0.");
        RuleFor(x => x.WardId)
            .GreaterThan(0).WithMessage("Ward ID must be greater than 0.");
        RuleFor(x => x.AddressType)
            .IsInEnum().WithMessage("Address type is invalid. It must be one of the defined enum values.");
        RuleFor(x => x.FullAddress)
            .NotEmpty().WithMessage("Full address is required.")
            .MaximumLength(500).WithMessage("Full address cannot exceed 500 characters.");

        RuleFor(x => x.StreetAddress)
            .NotEmpty().WithMessage("Street address is required.")
            .MaximumLength(200).WithMessage("Street address cannot exceed 200 characters.");

    }
}
