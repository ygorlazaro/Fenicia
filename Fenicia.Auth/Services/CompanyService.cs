using AutoMapper;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;

namespace Fenicia.Auth.Services;

public class CompanyService(
    IMapper mapper,
    ILogger<CompanyService> logger,
    ICompanyRepository companyRepository,
    IUserRoleService userRoleService) : ICompanyService
{
    public async Task<CompanyResponse?> GetByCnpjAsync(string cnpj)
    {
        logger.LogInformation("Getting company {cnpj}", [cnpj]);
        var company = await companyRepository.GetByCnpjAsync(cnpj);

        return company is null ? null : mapper.Map<CompanyResponse>(company);
    }

    public async Task<List<CompanyResponse>> GetByUserIdAsync(Guid userId, int page = 1, int perPage = 10)
    {
        logger.LogInformation("Getting companies by user {userId}", [userId]);
        var companies = await companyRepository.GetByUserIdAsync(userId, page, perPage);
        var response = mapper.Map<List<CompanyResponse>>(companies);

        return response;
    }

    public async Task<CompanyResponse?> PatchAsync(Guid companyId, Guid userId, CompanyRequest company)
    {
        logger.LogInformation("Patching company {companyId}", [companyId]);
        var hasAdminRole = await userRoleService.HasRoleAsync(userId, companyId, "Admin");

        if (!hasAdminRole)
        {
            logger.LogWarning("User {userId} does not have admin role", [userId]);
            throw new UnauthorizedAccessException(TextConstants.PermissionDenied);
        }

        var companyToUpdate = mapper.Map<CompanyRequest, CompanyModel>(company);

        companyToUpdate.Id = companyId;

        var response = await companyRepository.PatchAsync(companyToUpdate);

        return response is null ? null : mapper.Map<CompanyResponse>(response);
    }

    public async Task<int> CountByUserIdAsync(Guid userId)
    {
        return await companyRepository.CountByUserIdAsync(userId);
    }
}