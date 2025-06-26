using System.Net;

using AutoMapper;

using Fenicia.Auth.Domains.Company.Data;
using Fenicia.Auth.Domains.DataCache;
using Fenicia.Auth.Domains.UserRole.Logic;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.Company.Logic;

/// <summary>
/// Service responsible for managing company-related operations.
/// </summary>
/// <summary>
/// Service responsible for managing company-related operations.
/// </summary>
/// <param name="mapper">The AutoMapper instance for object mapping.</param>
/// <param name="logger">The logger instance for this service.</param>
/// <param name="companyRepository">The repository for company data access.</param>
/// <param name="userRoleService">The service for handling user roles.</param>
/// <param name="dataCacheService">The service for handling data caching.</param>
public class CompanyService(
    IMapper mapper,
    ILogger<CompanyService> logger,
    ICompanyRepository companyRepository,
    IUserRoleService userRoleService,
    IDataCacheService dataCacheService
) : ICompanyService
{
    /// <summary>
    /// Retrieves a company by its CNPJ (Brazilian company registration number).
    /// </summary>
    /// <param name="cnpj">The CNPJ of the company to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>ApiResponse containing the company information if found.</returns>
    public async Task<ApiResponse<CompanyResponse>> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting company with CNPJ: {cnpj}", cnpj);

            var cached = await dataCacheService.GetAsync<ApiResponse<CompanyResponse>>($"company:{cnpj}");

            if (cached is not null)
            {
                logger.LogInformation("Company with CNPJ: {cnpj} returned from cache", cnpj);
                return cached;
            }

            logger.LogInformation("Fetching company with CNPJ: {cnpj} from repository", cnpj);
            var company = await companyRepository.GetByCnpjAsync(cnpj, cancellationToken);

            if (company is null)
            {
                logger.LogWarning("Company with CNPJ: {cnpj} not found", cnpj);
                return new ApiResponse<CompanyResponse>(
                    null,
                    HttpStatusCode.NotFound,
                    TextConstants.ItemNotFound
                );
            }

            var response = new ApiResponse<CompanyResponse>(mapper.Map<CompanyResponse>(company));

            logger.LogInformation("Caching company data for CNPJ: {cnpj}", cnpj);
            await dataCacheService.SetAsync($"company:{cnpj}", response, TimeSpan.FromHours(1));

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting company with CNPJ: {cnpj}", cnpj);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a paginated list of companies associated with a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <param name="page">The page number for pagination (default is 1).</param>
    /// <param name="perPage">The number of items per page (default is 10).</param>
    /// <returns>ApiResponse containing a list of companies associated with the user.</returns>
    public async Task<ApiResponse<List<CompanyResponse>>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken,
        int page = 1,
        int perPage = 10
    )
    {
        try
        {
            logger.LogInformation("Getting companies for user: {userId}, page: {page}, items per page: {perPage}", userId, page, perPage);

            var cached = await dataCacheService.GetAsync<ApiResponse<List<CompanyResponse>>>($"company-userid:{userId}");

            if (cached is not null)
            {
                logger.LogInformation("Companies for user: {userId} returned from cache", userId);
                return cached;
            }

            logger.LogInformation("Fetching companies for user: {userId} from repository", userId);
            var companies = await companyRepository.GetByUserIdAsync(userId, cancellationToken, page, perPage);
            var response = new ApiResponse<List<CompanyResponse>>(mapper.Map<List<CompanyResponse>>(companies));

            logger.LogInformation("Caching companies data for user: {userId}", userId);
            await dataCacheService.SetAsync($"company-userid:{userId}", response, TimeSpan.FromHours(1));

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting companies for user: {userId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Updates specific fields of a company.
    /// </summary>
    /// <param name="companyId">The ID of the company to update.</param>
    /// <param name="userId">The ID of the user performing the update.</param>
    /// <param name="company">The company update request containing the fields to update.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>ApiResponse containing the updated company information.</returns>
    public async Task<ApiResponse<CompanyResponse>> PatchAsync(
        Guid companyId,
        Guid userId,
        CompanyUpdateRequest company,
        CancellationToken cancellationToken
    )
    {
        try
        {
            logger.LogInformation("Attempting to patch company: {companyId} by user: {userId}", companyId, userId);

            var existing = await companyRepository.CheckCompanyExistsAsync(companyId, cancellationToken);

            if (!existing)
            {
                logger.LogWarning("Company {companyId} not found", companyId);
                return new ApiResponse<CompanyResponse>(
                    null,
                    HttpStatusCode.NotFound,
                    TextConstants.ItemNotFound
                );
            }

            logger.LogInformation("Checking admin role for user: {userId} in company: {companyId}", userId, companyId);
            var hasAdminRole = await userRoleService.HasRoleAsync(userId, companyId, "Admin", cancellationToken);

            if (!hasAdminRole.Data)
            {
                logger.LogWarning("User: {userId} lacks admin role for company: {companyId}", userId, companyId);
                return new ApiResponse<CompanyResponse>(
                    null,
                    HttpStatusCode.Unauthorized,
                    TextConstants.PermissionDenied
                );
            }

            logger.LogInformation("Updating company: {companyId}", companyId);
            var companyToUpdate = mapper.Map<CompanyModel>(company);
            companyToUpdate.Id = companyId;

            var updatedCompany = companyRepository.PatchAsync(companyToUpdate);
            var saved = await companyRepository.SaveAsync(cancellationToken);

            if (saved == 0)
            {
                logger.LogWarning("Failed to save updates for company: {companyId}", companyId);
                return new ApiResponse<CompanyResponse>(
                    null,
                    HttpStatusCode.NotFound,
                    TextConstants.ItemNotFound
                );
            }

            logger.LogInformation("Successfully updated company: {companyId}", companyId);
            var response = mapper.Map<CompanyResponse>(updatedCompany);
            return new ApiResponse<CompanyResponse>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating company: {companyId}", companyId);
            throw;
        }
    }

    /// <summary>
    /// Counts the number of companies associated with a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>ApiResponse containing the count of companies associated with the user.</returns>
    public async Task<ApiResponse<int>> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Counting companies for user: {userId}", userId);
            var response = await companyRepository.CountByUserIdAsync(userId, cancellationToken);
            logger.LogInformation("Found {count} companies for user: {userId}", response, userId);

            return new ApiResponse<int>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while counting companies for user: {userId}", userId);
            throw;
        }
    }
}
