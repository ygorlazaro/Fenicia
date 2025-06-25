using System.Net;

using AutoMapper;

using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.Company;

public class CompanyService(
    IMapper mapper,
    ILogger<CompanyService> logger,
    ICompanyRepository companyRepository,
    IUserRoleService userRoleService
) : ICompanyService
{
    public async Task<ApiResponse<CompanyResponse>> GetByCnpjAsync(string cnpj)
    {
        logger.LogInformation("Getting company {cnpj}", cnpj);
        var company = await companyRepository.GetByCnpjAsync(cnpj);

        if (company is null)
        {
            return new ApiResponse<CompanyResponse>(
                null,
                HttpStatusCode.NotFound,
                TextConstants.ItemNotFound
            );
        }

        var response = mapper.Map<CompanyResponse>(company);

        return new ApiResponse<CompanyResponse>(response);
    }

    public async Task<ApiResponse<List<CompanyResponse>>> GetByUserIdAsync(
        Guid userId,
        int page = 1,
        int perPage = 10
    )
    {
        logger.LogInformation("Getting companies by user {userId}", userId);
        var companies = await companyRepository.GetByUserIdAsync(userId, page, perPage);
        var response = mapper.Map<List<CompanyResponse>>(companies);

        return new ApiResponse<List<CompanyResponse>>(response);
    }

    public async Task<ApiResponse<CompanyResponse>> PatchAsync(
        Guid companyId,
        Guid userId,
        CompanyUpdateRequest company
    )
    {
        logger.LogInformation("Patching company {companyId}", companyId);

        var existing = await companyRepository.CheckCompanyExistsAsync(companyId);

        if (!existing)
        {
            logger.LogWarning("Company {companyId} does not exist", companyId);

            return new ApiResponse<CompanyResponse>(
                null,
                HttpStatusCode.NotFound,
                TextConstants.ItemNotFound
            );
        }

        var hasAdminRole = await userRoleService.HasRoleAsync(userId, companyId, "Admin");

        if (!hasAdminRole.Data)
        {
            logger.LogWarning("User {userId} does not have admin role", userId);

            return new ApiResponse<CompanyResponse>(
                null,
                HttpStatusCode.Unauthorized,
                TextConstants.PermissionDenied
            );
        }

        var companyToUpdate = mapper.Map<CompanyModel>(company);

        companyToUpdate.Id = companyId;

        var updatedCompany = companyRepository.PatchAsync(companyToUpdate);
        var saved = await companyRepository.SaveAsync();

        if (saved == 0)
        {
            return new ApiResponse<CompanyResponse>(
                null,
                HttpStatusCode.NotFound,
                TextConstants.ItemNotFound
            );
        }

        var response = mapper.Map<CompanyResponse>(updatedCompany);

        return new ApiResponse<CompanyResponse>(response);
    }

    public async Task<ApiResponse<int>> CountByUserIdAsync(Guid userId)
    {
        var response = await companyRepository.CountByUserIdAsync(userId);

        return new ApiResponse<int>(response);
    }
}
