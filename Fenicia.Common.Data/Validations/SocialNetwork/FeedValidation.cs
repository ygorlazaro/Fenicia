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