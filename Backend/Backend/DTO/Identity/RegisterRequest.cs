using Backend.Data.Domain.Users.Enum;
using FluentValidation;

namespace Backend.DTO.Identity
{
    public class RegisterRequest
    {

        public string Username { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public Gender Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

    }

    public class RegisterModelValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterModelValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender value.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .Must(BeAValidDate).WithMessage("Date of birth cannot be in the future.")
                .Must(BeReasonable).WithMessage("Date of birth is too far in the past.");
        }

        private bool BeAValidDate(DateTime date)
        {
            return date <= DateTime.Today;
        }

        private bool BeReasonable(DateTime date)
        {
            return date >= DateTime.Today.AddYears(-150);
        }
    }
}
