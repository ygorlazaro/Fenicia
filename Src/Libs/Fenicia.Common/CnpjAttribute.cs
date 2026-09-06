using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class CnpjAttribute : ValidationAttribute
{
    private static readonly int[] CnpjWeightsFirst = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
    private static readonly int[] CnpjWeightsSecond = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

    public CnpjAttribute()
    {
    }

    public CnpjAttribute(string errorMessage)
        : base(errorMessage)
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var cnpj = value.ToString() ?? string.Empty;
        cnpj = new string([.. cnpj.Where(char.IsDigit)]);

        if (cnpj.Length != 14)
        {
            return new ValidationResult("CNPJ inválido.");
        }

        if (!IsDigitsSame(cnpj))
        {
            return new ValidationResult("CNPJ inválido.");
        }

        var firstCheckDigit = CalculateCheckDigit(cnpj[..12], CnpjWeightsFirst);
        var secondCheckDigit = CalculateCheckDigit(cnpj[..13], CnpjWeightsSecond);

        var firstDigit = cnpj[12].ToString();
        var secondDigit = cnpj[13].ToString();

        return firstCheckDigit == firstDigit && secondCheckDigit == secondDigit
            ? ValidationResult.Success
            : new ValidationResult("CNPJ inválido.");
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
