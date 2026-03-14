namespace Fenicia.Auth.Domains.Company.Commands;

/// <summary>
///     Command to update an existing company's information.
/// </summary>
/// <remarks>
///     Used by administrators to modify company details such as name.
/// </remarks>
public sealed record UpdateCompanyCommand(Guid CompanyId, Guid UserId, string Name);