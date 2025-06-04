using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Common;

namespace Fenicia.Auth.Services.Interfaces;

public interface IUserService
{
    Task<ServiceResponse<UserResponse>> GetForLoginAsync(TokenRequest request);
    Task<ServiceResponse<UserResponse>> CreateNewUserAsync(UserRequest request);
    Task<ServiceResponse<bool>> ExistsInCompanyAsync(Guid userId, Guid companyId);
    Task<ServiceResponse<UserResponse>> GetUserForRefreshAsync(Guid userId);
}