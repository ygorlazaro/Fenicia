using Fenicia.Common.Data.Requests.Basic;

using FluentValidation;

namespace Fenicia.Common.Data.Validations.Basic;

public class PersonValidation : AbstractValidator<PersonRequest>
{
    public PersonValidation()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .WithMessage("Name cannot be null")
            .MaximumLength(50)
            .WithMessage("Name cannot exceed 50 characters");

        RuleFor(x => x.Cpf)
            .IsValidCPF()
            .WithMessage("CPF invalid");
        
        RuleFor(x => x.City)
            .NotNull()
            .WithMessage("City cannot be null")
            .MaximumLength(50)
            .WithMessage("City cannot exceed 50 characters");
        
        RuleFor(x => x.Complement)
            .MaximumLength(50)
            .WithMessage("Complement cannot exceed 50 characters");
        
        RuleFor(x => x.Neighborhood)
            .NotNull()
            .WithMessage("Neighborhood cannot be null")
            .MaximumLength(50)
            .WithMessage("Neighborhood cannot exceed 50 characters");
        
        RuleFor(x => x.Number)
            .NotNull()
            .WithMessage("Number cannot be null")
            .MaximumLength(50)
            .WithMessage("Number cannot exceed 50 characters");
        
        RuleFor(x => x.StateId)
            .NotNull()
            .WithMessage("StateId cannot be null")
            .NotEmpty()
            .WithMessage("StateId cannot be empty");
        
        RuleFor(x => x.Street)
            .NotNull()
            .WithMessage("Street cannot be null")
            .MaximumLength(50)
            .WithMessage("Street cannot exceed 50 characters");

        RuleFor(x => x.ZipCode)
            .NotNull()
            .WithMessage("ZipCode cannot be null")
            .MaximumLength(8)
            .WithMessage("ZipCode cannot exceed 8 characters");
        
        RuleFor(x => x.PhoneNumber)
            .NotNull()
            .WithMessage("PhoneNumber cannot be null")
            .MaximumLength(50)
            .WithMessage("PhoneNumber cannot exceed 20 characters");
    }
}