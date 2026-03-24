using FluentValidation;
using WDA.Shared;

namespace WDA.Application.Users.Commands.CreateUserCommand;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(u => u.CreateUserDto.FirstName)
            .NotEmpty()
            .MinimumLength(Constants.FirstNameMinTextLength)
            .MaximumLength(Constants.FirstNameMaxTextLength);

        RuleFor(u => u.CreateUserDto.LastName)
            .NotEmpty()
            .MinimumLength(Constants.LastNameMinTextLength)
            .MaximumLength(Constants.LastNameMaxTextLength);

        RuleFor(u => u.CreateUserDto.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(Constants.EmailMaxTextLength);

        RuleFor(u => u.CreateUserDto.Password)
            .NotEmpty()
            .MinimumLength(Constants.PasswordMinTextLength)
            .MaximumLength(Constants.PasswordMaxTextLength)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    }
}
