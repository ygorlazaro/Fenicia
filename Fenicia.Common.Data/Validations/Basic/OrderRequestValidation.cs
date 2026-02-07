using Fenicia.Common.Data.Requests.Basic;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.Basic;

public class OrderRequestValidation : AbstractValidator<OrderRequest>
{
    public OrderRequestValidation()
    {
        RuleFor(x => x.CustomerId)
            .NotNull()
            .WithMessage("CustomerId cannot be null");

        RuleFor(x => x.Details)
            .NotNull()
            .WithMessage("Details cannot be null");

        RuleFor(x => x.SaleDate)
            .NotNull()
            .WithMessage("SaleDate cannot be null")
            .Must(x => DateTime.Now.AddDays(-7) < x.Date)
            .WithMessage("SaleDate cannot be in the future");

        RuleFor(x => x.Status)
            .NotNull()
            .WithMessage("Status cannot be null")
            .NotEmpty()
            .WithMessage("Status cannot be empty");
    }
}