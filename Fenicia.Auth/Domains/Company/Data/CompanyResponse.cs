namespace Fenicia.Auth.Domains.Company.Data;

using System.ComponentModel.DataAnnotations;

/// <summary>
///     Response model containing company information returned by the API
/// </summary>
/// <remarks>
///     This class represents the data structure used to return company information to clients.
///     It includes all essential company details such as identification, registration, and localization preferences.
/// </remarks>
public class CompanyResponse
{
    /// <summary>
    ///     The unique identifier of the company
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    /// <summary>
    ///     Gets or sets the unique identifier of the company
    /// </summary>
    /// <remarks>
    ///     This is the primary key used to identify the company in the system
    /// </remarks>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    [Required(ErrorMessage = "Company ID is required")]
    [Display(Name = "Company ID")]
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the name of the company
    /// </summary>
    /// <remarks>
    ///     The company name must not exceed 50 characters
    /// </remarks>
    /// <example>Acme Corporation</example>
    [Required(ErrorMessage = "Company name is required")]
    [MaxLength(length: 50, ErrorMessage = "Company name must not exceed 50 characters")]
    [Display(Name = "Company Name")]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the CNPJ (Brazilian company registration number)
    /// </summary>
    /// <remarks>
    ///     CNPJ must contain exactly 14 numeric characters without any special characters
    /// </remarks>
    /// <example>12345678000199</example>
    [Required(ErrorMessage = "CNPJ is required")]
    [MaxLength(length: 14, ErrorMessage = "CNPJ must not exceed 14 characters")]
    [RegularExpression(pattern: @"^\d{14}$", ErrorMessage = "CNPJ must contain exactly 14 numeric characters")]
    [Display(Name = "CNPJ")]
    public string Cnpj { get; set; } = null!;

    /// <summary>
    ///     The preferred language for the company
    /// </summary>
    /// <example>pt-BR</example>
    /// <summary>
    ///     Gets or sets the preferred language for the company
    /// </summary>
    /// <remarks>
    ///     The language code should follow the ISO 639-1 standard combined with ISO 3166-1 alpha-2 country code
    /// </remarks>
    /// <example>pt-BR</example>
    [Required(ErrorMessage = "Language is required")]
    [MaxLength(length: 5, ErrorMessage = "Language code must not exceed 5 characters")]
    [Display(Name = "Preferred Language")]
    public string Language { get; set; } = null!;

    /// <summary>
    ///     The timezone of the company
    /// </summary>
    /// <example>America/Sao_Paulo</example>
    /// <summary>
    ///     Gets or sets the timezone of the company
    /// </summary>
    /// <remarks>
    ///     The timezone should be in IANA Time Zone Database format
    /// </remarks>
    /// <example>America/Sao_Paulo</example>
    [Required(ErrorMessage = "Timezone is required")]
    [MaxLength(length: 50, ErrorMessage = "Timezone must not exceed 50 characters")]
    [Display(Name = "Time Zone")]
    public string TimeZone { get; set; } = null!;
}
