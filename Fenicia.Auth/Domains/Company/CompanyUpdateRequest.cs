namespace Fenicia.Auth.Domains.Company;

/// <summary>
/// Request model for company update
/// </summary>
public class CompanyUpdateRequest
{
    /// <summary>
    /// The name of the company
    /// </summary>
    /// <example>Acme Corporation</example>
    public string Name { get; set; } = null!;

    public string Timezone { get; set; } = null!;
}
