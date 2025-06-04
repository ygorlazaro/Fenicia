using Fenicia.Auth.Contexts.Models;

namespace Fenicia.Auth.Repositories.Interfaces;

public interface ICompanyRepository
{
    Task<bool> CheckCompanyExistsAsync(Guid companyId);
    Task<bool> CheckCompanyExistsAsync(string cnpj);
    CompanyModel Add(CompanyModel company);
    Task<int> SaveAsync();
    Task<CompanyModel?> GetByCnpjAsync(string cnpj);
    Task<List<CompanyModel>> GetByUserIdAsync(Guid userId, int page = 1, int perPage = 10);
    CompanyModel PatchAsync(CompanyModel company);
    Task<int> CountByUserIdAsync(Guid userId);
}