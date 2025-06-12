using FluentValidation;

namespace Backend.DTO.Roles;

public class UpdateRoleRequest
{
    public int Id { get; set; }
    public string FriendlyName { get; set; }
    public string SystemName { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFreeShipping { get; set; } = false;
}

public class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID không hợp lệ.");
        RuleFor(x => x.FriendlyName)
            .NotEmpty()
            .WithMessage("Tên hiển thị không được để trống.");


        RuleFor(x => x.IsActive)
            .NotNull()
            .WithMessage("Trạng thái hoạt động không được để trống.");
    }
}
