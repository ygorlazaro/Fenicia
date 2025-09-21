namespace Fenicia.Auth.Domains.UserRole;

using Common;

using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Responses;

public class UserRoleService : IUserRoleService
{
    private readonly ILogger<UserRoleService> logger;
    private readonly IUserRoleRepository userRoleRepository;

    public UserRoleService(ILogger<UserRoleService> logger, IUserRoleRepository userRoleRepository)
    {
        this.logger = logger;
        this.userRoleRepository = userRoleRepository;
    }

    public async Task<ApiResponse<string[]>> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Starting to retrieve roles for user {UserID}", userId);

            var roles = await this.userRoleRepository.GetRolesByUserAsync(userId, cancellationToken);

            this.logger.LogInformation("Successfully retrieved {RoleCount} roles for user {UserID}", roles.Length, userId);
            return new ApiResponse<string[]>(roles);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error retrieving roles for user {UserID}", userId);
            throw;
        }
    }

    public async Task<ApiResponse<List<CompanyResponse>>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Starting to retrieve companies for user {UserID}", userId);

            var userRoles = await this.userRoleRepository.GetUserCompaniesAsync(userId, cancellationToken);

            this.logger.LogInformation("Successfully retrieved companies for user {UserID}", userId);

            return new ApiResponse<List<CompanyResponse>>(CompanyModel.Convert(userRoles));
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error retrieving companies for user {UserID}", userId);
            throw;
        }
    }

    public async Task<ApiResponse<bool>> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Checking if user {UserID} has role {Role} in company {CompanyID}", userId, role, companyId);

            var response = await this.userRoleRepository.HasRoleAsync(userId, companyId, role, cancellationToken);

            this.logger.LogInformation("Role check completed for user {UserID}. Has role {Role}: {HasRole}", userId, role, response);
            return new ApiResponse<bool>(response);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error checking role {Role} for user {UserID} in company {CompanyID}", role, userId, companyId);
            throw;
        }
    }
}
