using FluentValidation;
using WDA.Application.Dtos;
using WDA.Shared;

namespace WDA.Application.Users.Commands.CreateUserCommand;

public class CreateUserCommandValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserCommandValidator()
    {
        RuleFor(u => u.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MinimumLength(Constants.FirstNameMinTextLength)
            .WithMessage($"First name should have a minimum length of {Constants.FirstNameMinTextLength}.")
            .MaximumLength(Constants.FirstNameMaxTextLength)
            .WithMessage($"First name must not exceed {Constants.FirstNameMaxTextLength} characters.");

        RuleFor(u => u.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MinimumLength(Constants.LastNameMinTextLength)
            .WithMessage($"Last name should have a minimum length of {Constants.LastNameMinTextLength}.")
            .MaximumLength(Constants.LastNameMaxTextLength)
            .WithMessage($"Last name must not exceed {Constants.LastNameMaxTextLength} characters.");

        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Invalid email address format.")
            .MaximumLength(Constants.EmailMaxTextLength)
            .WithMessage($"Email address must not exceed {Constants.EmailMaxTextLength} characters.");

        RuleFor(u => u.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(Constants.PasswordMinTextLength)
            .WithMessage($"Password should have a minimum length of {Constants.PasswordMinTextLength}.")
            .MaximumLength(Constants.PasswordMaxTextLength)
            .WithMessage($"Password must not exceed {Constants.PasswordMaxTextLength} characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    }
}
