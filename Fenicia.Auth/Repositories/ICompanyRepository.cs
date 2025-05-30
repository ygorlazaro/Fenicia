namespace Fenicia.Auth.Services;

public interface ICompanyRepository
{
    Task<bool> CheckCompanyExistsAsync(string cnpj);
}