using Backend.Data.Domain.Users.Enum;
using FluentValidation;

namespace Backend.DTO.Users;

public class UpdateUserRequest
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public Gender Gender { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public bool IsActive { get; set; }

    public string Notes { get; set; } = string.Empty;

    public List<int> RoleIds { get; set; } = new List<int>();
    public string Password { get; set; }

}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("User ID must be greater than 0.");


        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not valid.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[0-9]{9,15}$").WithMessage("Phone number format is invalid.");

        RuleFor(x => x.IsActive)
            .NotNull().WithMessage("Active status is required.");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.");

        RuleFor(x => x.RoleIds)
            .NotNull()
            .Must(x => x.Any()).WithMessage("At least one role must be selected.");
    }
}

