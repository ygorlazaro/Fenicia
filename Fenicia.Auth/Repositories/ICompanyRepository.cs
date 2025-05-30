using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Requests;

namespace Fenicia.Auth.Repositories;

public interface ICompanyRepository
{
    Task<bool> CheckCompanyExistsAsync(string cnpj);
    CompanyModel Add(CompanyModel company);
    Task<int> SaveAsync();
    Task<CompanyModel?> GetByCnpjAsync(string cnpj);
    Task<List<CompanyModel>> GetByUserIdAsync(Guid userId);
}