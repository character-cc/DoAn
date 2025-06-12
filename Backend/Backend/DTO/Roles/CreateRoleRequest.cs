using FluentValidation;

namespace Backend.DTO.Roles;

public class CreateRoleRequest
{
    public string FriendlyName { get; set; }
    public string SystemName { get; set; }
    public bool IsActive { get; set; } = true;

    public bool IsFreeShipping { get; set; } = false;


}

public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleFor(x => x.FriendlyName)
            .NotEmpty()
            .WithMessage("Tên hiển thị không được để trống.");
        RuleFor(x => x.SystemName)
            .NotEmpty()
            .WithMessage("Tên hệ thống không được để trống.")
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("Tên hệ thống chỉ được chứa chữ cái, số và dấu gạch dưới.");
        RuleFor(x => x.IsActive)
            .NotNull()
            .WithMessage("Trạng thái hoạt động không được để trống.");
    }
}
