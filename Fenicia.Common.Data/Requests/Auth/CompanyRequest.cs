namespace Fenicia.Common.Data.Requests.Auth;

public sealed class CompanyRequest
{
    public string Name { get; set; } = null!;

    public string Cnpj { get; set; } = null!;
}