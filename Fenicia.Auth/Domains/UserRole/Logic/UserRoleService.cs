namespace Fenicia.Auth.Domains.UserRole.Logic;

using Common;

public class UserRoleService : IUserRoleService
{
    private readonly ILogger<UserRoleService> _logger;
    private readonly IUserRoleRepository _userRoleRepository;

    public UserRoleService(ILogger<UserRoleService> logger, IUserRoleRepository userRoleRepository)
    {
        _logger = logger;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<ApiResponse<string[]>> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting to retrieve roles for user {UserId}", userId);

            var roles = await _userRoleRepository.GetRolesByUserAsync(userId, cancellationToken);

            _logger.LogInformation("Successfully retrieved {RoleCount} roles for user {UserId}", roles.Length, userId);
            return new ApiResponse<string[]>(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles for user {UserId}", userId);
            throw;
        }
    }

    public async Task<ApiResponse<bool>> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Checking if user {UserId} has role {Role} in company {CompanyId}", userId, role, companyId);

            var response = await _userRoleRepository.HasRoleAsync(userId, companyId, role, cancellationToken);

            _logger.LogInformation("Role check completed for user {UserId}. Has role {Role}: {HasRole}", userId, role, response);
            return new ApiResponse<bool>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role {Role} for user {UserId} in company {CompanyId}", role, userId, companyId);
            throw;
        }
    }
}
