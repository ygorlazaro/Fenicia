using Fenicia.Common.Data.Requests.SocialNetwork;
using Fenicia.Common.Data.Responses.SocialNetwork;

namespace Fenicia.Module.SocialNetwork.Domains.User;

public interface IUserService
{
    Task<UserResponse> AddAsync(UserRequest request, CancellationToken ct);

    Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<UserResponse?> UpdateAsync(Guid id, UserRequest request, CancellationToken ct);

    Task DeleteAsync(Guid id, CancellationToken ct);
}
