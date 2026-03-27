using FluentValidation;
using WDA.Shared;

namespace WDA.Application.Users.Commands.RegisterUserCommand;

public class RegisterCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(u => u.RegisterUserDto.FirstName)
            .NotEmpty()
            .MinimumLength(Constants.FirstNameMinTextLength)
            .MaximumLength(Constants.FirstNameMaxTextLength);

        RuleFor(u => u.RegisterUserDto.LastName)
            .NotEmpty()
            .MinimumLength(Constants.LastNameMinTextLength)
            .MaximumLength(Constants.LastNameMaxTextLength);

        RuleFor(u => u.RegisterUserDto.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(Constants.EmailMaxTextLength);

        RuleFor(u => u.RegisterUserDto.Password)
            .NotEmpty()
            .MinimumLength(Constants.PasswordMinTextLength)
            .MaximumLength(Constants.PasswordMaxTextLength)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        RuleFor(u => u.RegisterUserDto.ConfirmedPassword)
            .NotEmpty()
            .Equal(u => u.RegisterUserDto.Password)
            .WithMessage("Passwords do not match.")
            .MinimumLength(Constants.PasswordMinTextLength)
            .MaximumLength(Constants.PasswordMaxTextLength)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    }
}