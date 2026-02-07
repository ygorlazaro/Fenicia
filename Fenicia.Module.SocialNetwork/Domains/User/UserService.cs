using Fenicia.Common;
using Fenicia.Common.Data.Mappers.SocialNetwork;
using Fenicia.Common.Data.Requests.SocialNetwork;
using Fenicia.Common.Data.Responses.SocialNetwork;
using Fenicia.Common.Exceptions;

namespace Fenicia.Module.SocialNetwork.Domains.User;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<UserResponse> AddAsync(UserRequest request, CancellationToken ct)
    {
        var user = UserMapper.Map(request);

        userRepository.Add(user);

        await userRepository.SaveChangesAsync(ct);

        return UserMapper.Map(user);
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(id, ct);

        return user is null ? null : UserMapper.Map(user);
    }

    public async Task<UserResponse?> UpdateAsync(Guid id, UserRequest request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(id, ct) ?? throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);
        var model = UserMapper.Map(request);

        userRepository.Update(model);

        await userRepository.SaveChangesAsync(ct);

        return UserMapper.Map(model);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        userRepository.Delete(id);

        await userRepository.SaveChangesAsync(ct);
    }
}