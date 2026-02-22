using System.Linq.Expressions;

using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Company.GetCompaniesByUser;

public sealed class CompanyListItemResponse
{
    public Guid Id { get; set; }

    public string Name { get; init; } = string.Empty;
    public string Cnpj { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}
