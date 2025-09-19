namespace Fenicia.Auth.Domains.Company;

using System.Net;

using Common;

using Fenicia.Common.Database.Models.Auth;
using Common.Database.Requests;
using Common.Database.Responses;
using UserRole;

public class CompanyService : ICompanyService
{
    private readonly ILogger<CompanyService> logger;
    private readonly ICompanyRepository companyRepository;
    private readonly IUserRoleService userRoleService;

    public CompanyService(ILogger<CompanyService> logger, ICompanyRepository companyRepository, IUserRoleService userRoleService)
    {
        this.logger = logger;
        this.companyRepository = companyRepository;
        this.userRoleService = userRoleService;
    }

    public async Task<ApiResponse<CompanyResponse>> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Fetching company with CNPJ: {cnpj} from repository", cnpj);
            var company = await companyRepository.GetByCnpjAsync(cnpj, cancellationToken);

            if (company is null)
            {
                logger.LogWarning("Company with CNPJ: {cnpj} not found", cnpj);
                return new ApiResponse<CompanyResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
            }

            var mapped = CompanyResponse.Convert(company);
            var response = new ApiResponse<CompanyResponse>(mapped);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting company with CNPJ: {cnpj}", cnpj);
            throw;
        }
    }

    public async Task<ApiResponse<List<CompanyResponse>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        try
        {
            logger.LogInformation("Getting companies for user: {userID}, page: {page}, items per page: {perPage}", userId, page, perPage);

            logger.LogInformation("Fetching companies for user: {userID} from repository", userId);
            var companies = await companyRepository.GetByUserIdAsync(userId, cancellationToken, page, perPage);
            var mapped = CompanyResponse.Convert(companies);
            var response = new ApiResponse<List<CompanyResponse>>(mapped);

            logger.LogInformation("Caching companies data for user: {userID}", userId);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting companies for user: {userID}", userId);
            throw;
        }
    }

    public async Task<ApiResponse<CompanyResponse?>> PatchAsync(Guid companyId, Guid userId, CompanyUpdateRequest company, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Attempting to patch company: {companyID} by user: {userID}", companyId, userId);

            var existing = await companyRepository.CheckCompanyExistsAsync(companyId, cancellationToken);

            if (!existing)
            {
                logger.LogWarning("Company {companyID} not found", companyId);
                return new ApiResponse<CompanyResponse?>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
            }

            logger.LogInformation("Checking admin role for user: {userID} in company: {companyID}", userId, companyId);
            var hasAdminRole = await userRoleService.HasRoleAsync(userId, companyId, "Admin", cancellationToken);

            if (!hasAdminRole.Data)
            {
                logger.LogWarning("User: {userID} lacks admin role for company: {companyID}", userId, companyId);
                return new ApiResponse<CompanyResponse?>(data: null, HttpStatusCode.Unauthorized, TextConstants.PermissionDenied);
            }

            logger.LogInformation("Updating company: {companyID}", companyId);
            var companyToUpdate = CompanyModel.Convert(company);
            companyToUpdate.Id = companyId;

            var updatedCompany = companyRepository.PatchAsync(companyToUpdate);
            var saved = await companyRepository.SaveAsync(cancellationToken);

            if (saved == 0)
            {
                logger.LogWarning("Failed to save updates for company: {companyID}", companyId);
                return new ApiResponse<CompanyResponse?>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
            }

            logger.LogInformation("Successfully updated company: {companyID}", companyId);
            var response = CompanyResponse.Convert(updatedCompany);
            return new ApiResponse<CompanyResponse?>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating company: {companyID}", companyId);
            throw;
        }
    }

    public async Task<ApiResponse<int>> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Counting companies for user: {userID}", userId);
            var response = await companyRepository.CountByUserIdAsync(userId, cancellationToken);
            logger.LogInformation("Found {count} companies for user: {userID}", response, userId);

            return new ApiResponse<int>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while counting companies for user: {userID}", userId);
            throw;
        }
    }

    public async Task<List<Guid>> GetCompaniesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await companyRepository.GetCompaniesAsync(userId, cancellationToken);
    }
}
