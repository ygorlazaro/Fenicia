using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Common;

namespace Fenicia.Auth.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponse<UserResponse>> GetForLoginAsync(TokenRequest request);
    Task<ApiResponse<UserResponse>> CreateNewUserAsync(UserRequest request);
    Task<ApiResponse<bool>> ExistsInCompanyAsync(Guid userId, Guid companyId);
    Task<ApiResponse<UserResponse>> GetUserForRefreshAsync(Guid userId);
}
