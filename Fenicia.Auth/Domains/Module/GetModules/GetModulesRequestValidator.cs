using FluentValidation;

namespace Fenicia.Auth.Domains.Module.GetModules;

public sealed class GetModulesRequestValidator : AbstractValidator<GetModulesRequest>
{
    public GetModulesRequestValidator()
    {
        RuleFor(m => m.Page).GreaterThanOrEqualTo(1);
        RuleFor(m => m.PerPage).GreaterThanOrEqualTo(1);
    }
}
