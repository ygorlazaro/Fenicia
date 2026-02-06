using Fenicia.Common.Data.Requests.SocialNetwork;
using Fenicia.Common.Data.Responses.SocialNetwork;

namespace Fenicia.Module.SocialNetwork.Domains.User;

public interface IUserService
{
    Task<UserResponse> AddAsync(UserRequest request, CancellationToken cancellationToken);

    Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<UserResponse?> UpdateAsync(Guid id, UserRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
