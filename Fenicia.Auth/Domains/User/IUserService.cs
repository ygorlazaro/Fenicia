using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Auth.Domains.User;

public interface IUserService
{
    Task<UserResponse> GetForLoginAsync(TokenRequest request, CancellationToken ct);

    Task<UserResponse> CreateNewUserAsync(UserRequest request, CancellationToken ct);

    Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken ct);

    Task<UserResponse> GetUserForRefreshAsync(Guid userId, CancellationToken ct);

    Task<UserResponse> GetUserIdFromEmailAsync(string email, CancellationToken ct);

    Task<UserResponse> ChangePasswordAsync(Guid userId, string password, CancellationToken t);
}