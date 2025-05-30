using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories.Interfaces;

namespace Fenicia.Auth.Services.Interfaces;

public class CompanyService(ICompanyRepository companyRepository) : ICompanyService
{
    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj)
    {
        return await companyRepository.GetByCnpjAsync(cnpj);
    }

    public async Task<List<CompanyModel>> GetByUserIdAsync(Guid userId)
    {
        return await companyRepository.GetByUserIdAsync(userId);
    }
}