using Fenicia.Common;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Requests.SocialNetwork;
using Fenicia.Common.Data.Responses.SocialNetwork;
using Fenicia.Common.Exceptions;

namespace Fenicia.Module.SocialNetwork.Domains.User;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<UserResponse> AddAsync(UserRequest request, CancellationToken ct)
    {
        var user = new UserModel(request);

        userRepository.Add(user);

        await userRepository.SaveChangesAsync(ct);

        return new UserResponse(user);
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(id, ct);

        return user is null ? null : new UserResponse(user);
    }

    public async Task<UserResponse?> UpdateAsync(Guid id, UserRequest request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(id, ct)
                   ?? throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);
        var model = new UserModel(request);

        userRepository.Update(model);

        await userRepository.SaveChangesAsync(ct);

        return new UserResponse(user);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await userRepository.DeleteAsync(id, ct);

        await userRepository.SaveChangesAsync(ct);
    }
}