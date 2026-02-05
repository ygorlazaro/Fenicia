using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Requests.Auth;
using Fenicia.Common.Database.Responses.Auth;

namespace Fenicia.Common.Database.Converters.Auth;

public static class CompanyConverter
{
    public static List<CompanyResponse> Convert(List<UserRoleModel> userRoles)
    {
        return [.. userRoles.Select(ur => new CompanyResponse
        {
            Id = ur.Company.Id,
            Name = ur.Company.Name,
            Cnpj = ur.Company.Cnpj,
            Role = ur.Role
        })];
    }

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

    public static CompanyModel Convert(CompanyUpdateRequest company)
    {
        return new CompanyModel { Name = company.Name, TimeZone = company.Timezone };
    }
}
