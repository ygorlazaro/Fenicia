using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Responses.Auth;

public class CompanyResponse
{
    public CompanyResponse(CompanyModel model)
    {
        this.Id = model.Id;
        this.Name = model.Name;
        this.Cnpj = model.Cnpj;
        this.Language = model.Language;
        this.TimeZone = model.TimeZone;
        this.Role = string.Empty;
    }

    public CompanyResponse(UserRoleModel model)
    {
        this.Id = model.Company.Id;
        this.Name = model.Company.Name;
        this.Cnpj = model.Company.Cnpj;
        this.Role = model.Role.Name;
        this.Language = string.Empty;
        this.TimeZone = string.Empty;
    }

    public CompanyResponse()
    {
        this.Id = Guid.Empty;
        this.Name = string.Empty;
        this.Cnpj = string.Empty;
        this.Language = string.Empty;
        this.TimeZone = string.Empty;
        this.Role = string.Empty;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Cnpj { get; set; }

    public string Language { get; set; }

    public string TimeZone { get; set; }

    public string Role { get; set; }
}
