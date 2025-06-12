using FluentValidation;

namespace Backend.DTO.Identity;

public class LoginRequest
{

    public string UserNameOrEmail { get; set; }  = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
    public string ReturnUrl { get; set; } = string.Empty;
}

   
public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.UserNameOrEmail)
            .NotEmpty().WithMessage("Username or Email is required.")
            .MaximumLength(100).WithMessage("Username or Email must not exceed 100 characters.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}