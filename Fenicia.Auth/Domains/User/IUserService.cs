using Fenicia.Common.Database.Requests.Auth;
using Fenicia.Common.Database.Responses.Auth;

namespace Fenicia.Auth.Domains.User;

public interface IUserService
{
    Task<UserResponse> GetForLoginAsync(TokenRequest request, CancellationToken cancellationToken);

    Task<UserResponse> CreateNewUserAsync(UserRequest request, CancellationToken cancellationToken);

    Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken);

    Task<UserResponse> GetUserForRefreshAsync(Guid userId, CancellationToken cancellationToken);

    Task<UserResponse> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken);

    Task<UserResponse> ChangePasswordAsync(Guid userId, string password, CancellationToken cancellationToken);
}
