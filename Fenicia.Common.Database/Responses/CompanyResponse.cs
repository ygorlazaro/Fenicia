using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Common.Database.Responses;

public class CompanyResponse
{
    public Guid Id
    {
        get; set;
    }

    public string Name { get; set; } = null!;

    public string Cnpj { get; set; } = null!;

    public string Language { get; set; } = null!;

    public string TimeZone { get; set; } = null!;

    public RoleModel Role { get; set; } = null!;

    public static List<CompanyResponse> Convert(List<UserRoleModel> userRoles)
    {
        return [.. userRoles.Select(ur => new CompanyResponse
        {
            Id = ur.Company.Id,
            Name = ur.Company.Name,
            Cnpj = ur.Company.Cnpj,
            Language = ur.Company.Language,
            TimeZone = ur.Company.TimeZone,
            Role = ur.Role
        })];
    }

    public override bool Equals(object? obj)
    {
        if (obj is not CompanyResponse other)
        {
            return false;
        }

        return Id == other.Id &&
               Name == other.Name &&
               Cnpj == other.Cnpj &&
               Language == other.Language &&
               TimeZone == other.TimeZone &&
               Equals(Role, other.Role);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Cnpj, Language, TimeZone, Role);
    }
}
