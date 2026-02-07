using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Responses.Auth;

public class CompanyResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Cnpj { get; set; } = null!;

    public string Language { get; set; } = null!;

    public string TimeZone { get; set; } = null!;

    public string Role { get; set; } = null!;

    public static List<CompanyResponse> Map(List<UserRoleModel> userRoles)
    {
        return [.. userRoles.Select(ur => new CompanyResponse
        {
            Id = ur.Company.Id,
            Name = ur.Company.Name,
            Cnpj = ur.Company.Cnpj,
            Language = ur.Company.Language,
            TimeZone = ur.Company.TimeZone,
            Role = ur.Role.Name
        })];
    }
}