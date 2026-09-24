using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.User;
using UserCompanyResponse = Fenicia.Common.DTOs.Auth.UserRole.UserCompanyResponse;

namespace Fenicia.Auth.Domains.User.Interfaces;

public interface IUserService
{
    Task<Pagination<List<UserResponse>>> GetAllAsync(
        UserRequest query,
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default);

    Task<UserResponse?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserModel?> FirstByEmailOrDefaultAsync(string email, CancellationToken cancellationToken = default);

    Task<List<UserCompanyResponse>> GetCompaniesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task EnsureCanAccessUserAsync(
        Guid loggedInUserId,
        Guid requestedUserId,
        Guid? companyId,
        CancellationToken cancellationToken = default);

    Task<UserResponse> CreateAsync(UserRequest request, CancellationToken cancellationToken = default);

    Task<UserResponse> UpdateAsync(UserRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<UserModel> UpdatePasswordAsync(
        Guid userId,
        string plainPassword,
        CancellationToken cancellationToken = default);
}
