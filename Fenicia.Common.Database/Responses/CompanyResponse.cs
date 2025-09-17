namespace Fenicia.Common.Database.Responses;

using Models.Auth;

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

    public static List<CompanyResponse> Convert(List<CompanyModel> companies)
    {
        return [.. companies.Select(CompanyResponse.Convert)];
    }
}
