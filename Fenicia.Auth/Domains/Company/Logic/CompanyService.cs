namespace Fenicia.Auth.Domains.Company.Logic;

using System.Net;

using Common;

using Fenicia.Common.Database.Models.Auth;
using Common.Database.Requests;
using Common.Database.Responses;

using UserRole.Logic;

public class CompanyService : ICompanyService
{
    private readonly ILogger<CompanyService> _logger;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserRoleService _userRoleService;

    public CompanyService(ILogger<CompanyService> logger, ICompanyRepository companyRepository, IUserRoleService userRoleService)
    {
        _logger = logger;
        _companyRepository = companyRepository;
        _userRoleService = userRoleService;
    }

    public async Task<ApiResponse<CompanyResponse>> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching company with CNPJ: {cnpj} from repository", cnpj);
            var company = await _companyRepository.GetByCnpjAsync(cnpj, cancellationToken);

            if (company is null)
            {
                _logger.LogWarning("Company with CNPJ: {cnpj} not found", cnpj);
                return new ApiResponse<CompanyResponse>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
            }

            var mapped = CompanyResponse.Convert(company);
            var response = new ApiResponse<CompanyResponse>(mapped);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting company with CNPJ: {cnpj}", cnpj);
            throw;
        }
    }

    public async Task<ApiResponse<List<CompanyResponse>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        try
        {
            _logger.LogInformation("Getting companies for user: {userId}, page: {page}, items per page: {perPage}", userId, page, perPage);

            _logger.LogInformation("Fetching companies for user: {userId} from repository", userId);
            var companies = await _companyRepository.GetByUserIdAsync(userId, cancellationToken, page, perPage);
            var mapped = CompanyResponse.Convert(companies);
            var response = new ApiResponse<List<CompanyResponse>>(mapped);

            _logger.LogInformation("Caching companies data for user: {userId}", userId);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting companies for user: {userId}", userId);
            throw;
        }
    }

    public async Task<ApiResponse<CompanyResponse?>> PatchAsync(Guid companyId, Guid userId, CompanyUpdateRequest company, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Attempting to patch company: {companyId} by user: {userId}", companyId, userId);

            var existing = await _companyRepository.CheckCompanyExistsAsync(companyId, cancellationToken);

            if (!existing)
            {
                _logger.LogWarning("Company {companyId} not found", companyId);
                return new ApiResponse<CompanyResponse?>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
            }

            _logger.LogInformation("Checking admin role for user: {userId} in company: {companyId}", userId, companyId);
            var hasAdminRole = await _userRoleService.HasRoleAsync(userId, companyId, "Admin", cancellationToken);

            if (!hasAdminRole.Data)
            {
                _logger.LogWarning("User: {userId} lacks admin role for company: {companyId}", userId, companyId);
                return new ApiResponse<CompanyResponse?>(data: null, HttpStatusCode.Unauthorized, TextConstants.PermissionDenied);
            }

            _logger.LogInformation("Updating company: {companyId}", companyId);
            var companyToUpdate = CompanyModel.Convert(company);
            companyToUpdate.Id = companyId;

            var updatedCompany = _companyRepository.PatchAsync(companyToUpdate);
            var saved = await _companyRepository.SaveAsync(cancellationToken);

            if (saved == 0)
            {
                _logger.LogWarning("Failed to save updates for company: {companyId}", companyId);
                return new ApiResponse<CompanyResponse?>(data: null, HttpStatusCode.NotFound, TextConstants.ItemNotFound);
            }

            _logger.LogInformation("Successfully updated company: {companyId}", companyId);
            var response = CompanyResponse.Convert(updatedCompany);
            return new ApiResponse<CompanyResponse?>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating company: {companyId}", companyId);
            throw;
        }
    }

    public async Task<ApiResponse<int>> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Counting companies for user: {userId}", userId);
            var response = await _companyRepository.CountByUserIdAsync(userId, cancellationToken);
            _logger.LogInformation("Found {count} companies for user: {userId}", response, userId);

            return new ApiResponse<int>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while counting companies for user: {userId}", userId);
            throw;
        }
    }
}
