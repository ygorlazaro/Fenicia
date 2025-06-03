using AutoMapper;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;

namespace Fenicia.Auth.Services;

public class CompanyService(IMapper mapper, ICompanyRepository companyRepository, IUserRoleService userRoleService) : ICompanyService
{
    public async Task<CompanyResponse?> GetByCnpjAsync(string cnpj)
    {
        var company =  await companyRepository.GetByCnpjAsync(cnpj);

        return company is null ? null : mapper.Map<CompanyResponse>(company);
    }

    public async Task<List<CompanyResponse>> GetByUserIdAsync(Guid userId)
    {
        var companies = await companyRepository.GetByUserIdAsync(userId);
        var response = mapper.Map<List<CompanyResponse>>(companies);

        return response;
    }

    public async Task<CompanyResponse?> PatchAsync(Guid companyId, Guid userId, CompanyRequest company)
    {
        var hasAdminRole = await userRoleService.HasRoleAsync(userId, companyId, "Admin");
        
        if (!hasAdminRole)
        {
            throw new UnauthorizedAccessException(TextConstants.PermissionDenied);
        }
        
        var companyToUpdate = mapper.Map<CompanyRequest, CompanyModel>(company);

        companyToUpdate.Id = companyId;
        
        var response = await companyRepository.PatchAsync(companyToUpdate);

        return response is null ? null : mapper.Map<CompanyResponse>(response);
    }
}