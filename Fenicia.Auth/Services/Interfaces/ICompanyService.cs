using Fenicia.Auth.Contexts.Models;

namespace Fenicia.Auth.Services.Interfaces;

public interface ICompanyService
{
    Task<CompanyModel?> GetByCnpjAsync(string cnpj);
    Task<List<CompanyModel>> GetByUserIdAsync(Guid userId);
}