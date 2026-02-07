using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Common.Data.Mappers.Auth;

public static class CompanyMapper
{
    public static List<CompanyResponse> Map(List<UserRoleModel> userRoles)
    {
        return
        [
            .. userRoles.Select(ur => new CompanyResponse
            {
                Id = ur.Company.Id,
                Name = ur.Company.Name,
                Cnpj = ur.Company.Cnpj,
                Role = ur.Role.Name
            })
        ];
    }

    public static CompanyResponse Map(CompanyModel company)
    {
        return new CompanyResponse
        {
            Id = company.Id,
            Name = company.Name,
            Cnpj = company.Cnpj,
            Language = company.Language,
            TimeZone = company.TimeZone,
            Role = string.Empty
        };
    }

    public static CompanyModel Map(CompanyUpdateRequest company)
    {
        return new CompanyModel { Name = company.Name, TimeZone = company.Timezone };
    }

    public static List<CompanyResponse> Map(List<CompanyModel> models)
    {
        return [.. models.Select(Map)];
    }

    public static CompanyModel Map(CompanyRequest request)
    {
        return new CompanyModel { Name = request.Name, Cnpj = request.Cnpj };
    }
}