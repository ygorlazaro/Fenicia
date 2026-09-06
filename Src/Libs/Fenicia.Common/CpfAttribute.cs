using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class CpfAttribute : ValidationAttribute
{
    private static readonly int[] CpfWeightsFirst = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
    private static readonly int[] CpfWeightsSecond = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

    public CpfAttribute()
    {
    }

    public CpfAttribute(string errorMessage)
        : base(errorMessage)
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var cpf = value.ToString() ?? string.Empty;
        cpf = new string([.. cpf.Where(char.IsDigit)]);

        if (cpf.Length != 11)
        {
            return new ValidationResult("CPF inválido.");
        }

        if (!IsDigitsSame(cpf))
        {
            return new ValidationResult("CPF inválido.");
        }

        var firstCheckDigit = CalculateCheckDigit(cpf[..9], CpfWeightsFirst);
        var secondCheckDigit = CalculateCheckDigit(cpf[..10], CpfWeightsSecond);

        var firstDigit = cpf[9].ToString();
        var secondDigit = cpf[10].ToString();

        return firstCheckDigit == firstDigit && secondCheckDigit == secondDigit
            ? ValidationResult.Success
            : new ValidationResult("CPF inválido.");
    }

    private static bool IsDigitsSame(string digits)
    {
        return digits.All(d => d == digits[0]);
    }

    private static string CalculateCheckDigit(string baseNumber, int[] weights)
    {
        var sum = baseNumber.Select((c, i) => int.Parse(c.ToString()) * weights[i]).Sum();
        var remainder = sum % 11;
        var digit = remainder < 2 ? 0 : 11 - remainder;
        return digit.ToString();
    }
}
