using Fenicia.Common.Data.Requests.SocialNetwork;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.SocialNetwork;

public class FeedValidation : AbstractValidator<FeedRequest>
{
    public FeedValidation()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Text cannot be empty")
            .MaximumLength(512)
            .WithMessage("Text cannot be longer than 512 characters");
    }
}

public class UserValidation : AbstractValidator<UserRequest>
{
    public UserValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name cannot be empty")
            .MaximumLength(50)
            .WithMessage("Name cannot be longer than 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty")
            .EmailAddress()
            .WithMessage("Email cannot be empty");

        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username cannot be empty")
            .MaximumLength(50)
            .WithMessage("Username cannot be longer than 50 characters");
    }
}