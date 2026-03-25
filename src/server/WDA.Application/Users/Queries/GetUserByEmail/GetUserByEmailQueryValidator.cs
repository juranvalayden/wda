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
            .MaximumLength(Constants.EmailMaxTextLength);
    }
}
