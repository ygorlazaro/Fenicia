namespace Fenicia.Auth.Domains.Company.Data;

using System.ComponentModel.DataAnnotations;

public class CompanyResponse
{
    [Required(ErrorMessage = "Company ID is required")]
    [Display(Name = "Company ID")]
    public Guid Id
    {
        get; set;
    }

    [Required(ErrorMessage = "Company name is required")]
    [MaxLength(length: 50, ErrorMessage = "Company name must not exceed 50 characters")]
    [Display(Name = "Company Name")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "CNPJ is required")]
    [MaxLength(length: 14, ErrorMessage = "CNPJ must not exceed 14 characters")]
    [RegularExpression(pattern: @"^\d{14}$", ErrorMessage = "CNPJ must contain exactly 14 numeric characters")]
    [Display(Name = "CNPJ")]
    public string Cnpj { get; set; } = null!;

    [Required(ErrorMessage = "Language is required")]
    [MaxLength(length: 5, ErrorMessage = "Language code must not exceed 5 characters")]
    [Display(Name = "Preferred Language")]
    public string Language { get; set; } = null!;

    [Required(ErrorMessage = "Timezone is required")]
    [MaxLength(length: 50, ErrorMessage = "Timezone must not exceed 50 characters")]
    [Display(Name = "Time Zone")]
    public string TimeZone { get; set; } = null!;

    public static CompanyResponse Convert(CompanyModel company)
    {
        return new CompanyResponse
        {
            Id = company.Id,
            Name = company.Name,
            Cnpj = company.Cnpj,
            Language = company.Language,
            TimeZone = company.TimeZone
        };
    }

    public static List<CompanyResponse> Convert(List<CompanyModel> companies)
    {
        return companies.Select(CompanyResponse.Convert).ToList();
    }
}
