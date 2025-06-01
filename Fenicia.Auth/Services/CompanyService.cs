using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Services.Interfaces;

namespace Fenicia.Auth.Services;

public class CompanyService(ICompanyRepository companyRepository, IUserRoleService userRoleService) : ICompanyService
{
    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj)
    {
        return await companyRepository.GetByCnpjAsync(cnpj);
    }

    public async Task<List<CompanyModel>> GetByUserIdAsync(Guid userId)
    {
        return await companyRepository.GetByUserIdAsync(userId);
    }

    public async Task<CompanyModel?> PatchAsync(Guid companyId, Guid userId, CompanyModel company)
    {
        var hasAdminRole = await userRoleService.HasRoleAsync(userId, companyId, "Admin");
        
        if (!hasAdminRole)
        {
            return null;
        }

        company.Id = companyId;
        
        var response = await companyRepository.PatchAsync(company);

        return response;
    }
}