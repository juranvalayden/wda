using FluentValidation;
using WDA.Shared;

namespace WDA.Application.Users.Queries.GetUserByEmail;

public class GetUserByEmailQueryValidator : AbstractValidator<GetUserByEmailQuery>
{
    public GetUserByEmailQueryValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Invalid email address format.")
            .MaximumLength(Constants.EmailMaxTextLength)
            .WithMessage($"Email address must not exceed {Constants.EmailMaxTextLength} characters.");
    }
}
