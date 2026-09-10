namespace Fenicia.Web.Components.Layout.Models;

public class UserCompanyItem
{
    public Guid Id { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string Cnpj { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}
