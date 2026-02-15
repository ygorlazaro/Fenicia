using FluentValidation;

namespace Fenicia.Auth.Domains.Company.UpdateCompany;

public sealed class UpdateCompanyValidator: AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.TimeZone).NotEmpty();
    }
}
