using FluentValidation;
using WDA.Shared;

namespace WDA.Application.Users.Commands.LoginUserCommand;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(u => u.LoginUserDto.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(Constants.EmailMaxTextLength);

        RuleFor(u => u.LoginUserDto.Password)
            .NotEmpty()
            .MinimumLength(Constants.PasswordMinTextLength)
            .MaximumLength(Constants.PasswordMaxTextLength)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    }
}
