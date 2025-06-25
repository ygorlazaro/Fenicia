using System.Net;

using AutoMapper;

using Fenicia.Auth.Domains.DataCache;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.Company;

public class CompanyService(
    IMapper mapper,
    ILogger<CompanyService> logger,
    ICompanyRepository companyRepository,
    IUserRoleService userRoleService,
    IDataCacheService dataCacheService
) : ICompanyService
{
    public async Task<ApiResponse<CompanyResponse>> GetByCnpjAsync(string cnpj)
    {
        logger.LogInformation("Getting company {cnpj}", cnpj);

        var cached = await dataCacheService.GetAsync<ApiResponse<CompanyResponse>>($"company:{cnpj}");

        if (cached is not null)
        {
            logger.LogInformation("Returned from cache");

            return cached;
        }

        var company = await companyRepository.GetByCnpjAsync(cnpj);

        if (company is null)
        {
            return new ApiResponse<CompanyResponse>(
                null,
                HttpStatusCode.NotFound,
                TextConstants.ItemNotFound
            );
        }

        var response = new ApiResponse<CompanyResponse>(mapper.Map<CompanyResponse>(company));

        await dataCacheService.SetAsync($"company:{cnpj}", response, TimeSpan.FromHours(1));

        return response;
    }

    public async Task<ApiResponse<List<CompanyResponse>>> GetByUserIdAsync(
        Guid userId,
        int page = 1,
        int perPage = 10
    )
    {
        logger.LogInformation("Getting companies by user {userId}", userId);

        var cached = await dataCacheService.GetAsync<ApiResponse<List<CompanyResponse>>>($"company-userid:{userId}");

        if (cached is not null)
        {
            logger.LogInformation("Returned from cache");

            return cached;
        }

        var companies = await companyRepository.GetByUserIdAsync(userId, page, perPage);
        var response = new ApiResponse<List<CompanyResponse>>(mapper.Map<List<CompanyResponse>>(companies));

        await dataCacheService.SetAsync($"company-userid:{userId}", response, TimeSpan.FromHours(1));

        return response;
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
