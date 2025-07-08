namespace Fenicia.Auth.Domains.Company.Logic;

using System.Net;

using Common;

using Data;

using DataCache;

using UserRole.Logic;

public class CompanyService(ILogger<CompanyService> logger, ICompanyRepository companyRepository, IUserRoleService userRoleService, IDataCacheService dataCacheService) : ICompanyService
{
    public async Task<ApiResponse<CompanyResponse>> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Getting company with CNPJ: {cnpj}", cnpj);

            var cached = await dataCacheService.GetAsync<ApiResponse<CompanyResponse>>($"company:{cnpj}");

            if (cached is not null)
            {
                logger.LogInformation(message: "Company with CNPJ: {cnpj} returned from cache", cnpj);
                return cached;
            }

            logger.LogInformation(message: "Fetching company with CNPJ: {cnpj} from repository", cnpj);
            var company = await companyRepository.GetByCnpjAsync(cnpj, cancellationToken);

            if (company is null)
            {
                logger.LogWarning(message: "Company with CNPJ: {cnpj} not found", cnpj);
                return new ApiResponse<CompanyResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
            }

            var mapped = CompanyResponse.Convert(company);
            var response = new ApiResponse<CompanyResponse>(mapped);

            logger.LogInformation(message: "Caching company data for CNPJ: {cnpj}", cnpj);
            await dataCacheService.SetAsync($"company:{cnpj}", response, TimeSpan.FromHours(hours: 1));

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error occurred while getting company with CNPJ: {cnpj}", cnpj);
            throw;
        }
    }

    public async Task<ApiResponse<List<CompanyResponse>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        try
        {
            logger.LogInformation(message: "Getting companies for user: {userId}, page: {page}, items per page: {perPage}", userId, page, perPage);

            var cached = await dataCacheService.GetAsync<ApiResponse<List<CompanyResponse>>>($"company-userid:{userId}");

            if (cached is not null)
            {
                logger.LogInformation(message: "Companies for user: {userId} returned from cache", userId);
                return cached;
            }

            logger.LogInformation(message: "Fetching companies for user: {userId} from repository", userId);
            var companies = await companyRepository.GetByUserIdAsync(userId, cancellationToken, page, perPage);
            var mapped = CompanyResponse.Convert(companies);
            var response = new ApiResponse<List<CompanyResponse>>(mapped);

            logger.LogInformation(message: "Caching companies data for user: {userId}", userId);
            await dataCacheService.SetAsync($"company-userid:{userId}", response, TimeSpan.FromHours(hours: 1));

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error occurred while getting companies for user: {userId}", userId);
            throw;
        }
    }

    public async Task<ApiResponse<CompanyResponse?>> PatchAsync(Guid companyId, Guid userId, CompanyUpdateRequest company, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Attempting to patch company: {companyId} by user: {userId}", companyId, userId);

            var existing = await companyRepository.CheckCompanyExistsAsync(companyId, cancellationToken);

            if (!existing)
            {
                logger.LogWarning(message: "Company {companyId} not found", companyId);
                return new ApiResponse<CompanyResponse?>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
            }

            logger.LogInformation(message: "Checking admin role for user: {userId} in company: {companyId}", userId, companyId);
            var hasAdminRole = await userRoleService.HasRoleAsync(userId, companyId, role: "Admin", cancellationToken);

            if (!hasAdminRole.Data)
            {
                logger.LogWarning(message: "User: {userId} lacks admin role for company: {companyId}", userId, companyId);
                return new ApiResponse<CompanyResponse?>(data: null, HttpStatusCode.Unauthorized, TextConstants.PermissionDenied);
            }

            logger.LogInformation(message: "Updating company: {companyId}", companyId);
            var companyToUpdate = CompanyModel.Convert(company);
            companyToUpdate.Id = companyId;

            var updatedCompany = companyRepository.PatchAsync(companyToUpdate);
            var saved = await companyRepository.SaveAsync(cancellationToken);

            if (saved == 0)
            {
                logger.LogWarning(message: "Failed to save updates for company: {companyId}", companyId);
                return new ApiResponse<CompanyResponse?>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
            }

            logger.LogInformation(message: "Successfully updated company: {companyId}", companyId);
            var response = CompanyResponse.Convert(updatedCompany);
            return new ApiResponse<CompanyResponse?>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error occurred while updating company: {companyId}", companyId);
            throw;
        }
    }

    public async Task<ApiResponse<int>> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Counting companies for user: {userId}", userId);
            var response = await companyRepository.CountByUserIdAsync(userId, cancellationToken);
            logger.LogInformation(message: "Found {count} companies for user: {userId}", response, userId);

            return new ApiResponse<int>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error occurred while counting companies for user: {userId}", userId);
            throw;
        }
    }
}
