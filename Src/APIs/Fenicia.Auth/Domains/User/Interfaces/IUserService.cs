using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.UserRole.DTOs;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.User.Interfaces;

public interface IUserService
{
    Task<Pagination<List<UserListItemResponse>>> GetAllAsync(GetAllUsersQuery query, CancellationToken cancellationToken = default);

    Task<GetUserByIdResponse?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<GetByEmailResponse?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<UserModel> FirstByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserModel?> FirstByEmailOrDefaultAsync(string email, CancellationToken cancellationToken = default);

    Task<UserModel> UpdatePasswordAsync(Guid userId, string plainPassword, CancellationToken cancellationToken = default);

    Task<GetUserForRefreshResponse> GetForRefreshAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<List<GetUserCompaniesResponse>> GetCompaniesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task EnsureCanAccessUserAsync(Guid loggedInUserId, Guid requestedUserId, Guid? companyId, CancellationToken cancellationToken = default);

    Task<CreateUserResponse> CreateAsync(CreateUserCommand command, CancellationToken cancellationToken = default);

    Task<CreateNewUserResponse> CreateNewAsync(CreateNewUserCommand command, CancellationToken cancellationToken = default);

    Task<UpdateUserResponse> UpdateAsync(UpdateUserCommand command, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UpdateUserPasswordResponse> UpdatePasswordAsync(UpdateUserPasswordCommand command, CancellationToken cancellationToken = default);

    Task<UpdatePasswordResponse> UpdateHashedPasswordAsync(UpdatePasswordCommand command, CancellationToken cancellationToken = default);
}
