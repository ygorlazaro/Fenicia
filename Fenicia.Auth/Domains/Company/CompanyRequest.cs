namespace Fenicia.Auth.Domains.Company;

/// <summary>
/// Request model for company creation
/// </summary>
public class CompanyRequest
{
    /// <summary>
    /// The name of the company
    /// </summary>
    /// <example>Acme Corporation</example>
    public string Name { get; set; } = null!;

    /// <summary>
    /// The CNPJ (Brazilian company registration number)
    /// </summary>
    /// <example>12345678000199</example>
    public string Cnpj { get; set; } = null!;
}
