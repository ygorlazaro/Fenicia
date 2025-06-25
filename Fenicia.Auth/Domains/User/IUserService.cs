using Fenicia.Auth.Domains.Token;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.User;

public interface IUserService
{
    Task<ApiResponse<UserResponse>> GetForLoginAsync(TokenRequest request);
    Task<ApiResponse<UserResponse>> CreateNewUserAsync(UserRequest request);
    Task<ApiResponse<bool>> ExistsInCompanyAsync(Guid userId, Guid companyId);
    Task<ApiResponse<UserResponse>> GetUserForRefreshAsync(Guid userId);
    Task<ApiResponse<UserResponse>> GetUserIdFromEmailAsync(string email);
    Task<ApiResponse<UserResponse>> ChangePasswordAsync(Guid userId, string password);
}
