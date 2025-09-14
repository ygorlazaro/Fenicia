namespace Fenicia.Auth.Domains.User.Logic;

using Common;
using Common.Database.Requests;
using Common.Database.Responses;

public interface IUserService
{
    Task<ApiResponse<UserResponse>> GetForLoginAsync(TokenRequest request, CancellationToken cancellationToken);

    Task<ApiResponse<UserResponse>> CreateNewUserAsync(UserRequest request, CancellationToken cancellationToken);

    Task<ApiResponse<bool>> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken);

    Task<ApiResponse<UserResponse>> GetUserForRefreshAsync(Guid userId, CancellationToken cancellationToken);

    Task<ApiResponse<UserResponse>> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken);

    Task<ApiResponse<UserResponse>> ChangePasswordAsync(Guid userId, string password, CancellationToken cancellationToken);
}
