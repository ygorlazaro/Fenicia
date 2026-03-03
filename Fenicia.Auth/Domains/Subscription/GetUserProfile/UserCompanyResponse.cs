namespace Fenicia.Auth.Domains.Subscription.GetUserProfile;

public class UserCompanyResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
}