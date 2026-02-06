using Fenicia.Common;
using Fenicia.Common.Data.Converters.SocialNetwork;
using Fenicia.Common.Data.Requests.SocialNetwork;
using Fenicia.Common.Data.Responses.SocialNetwork;
using Fenicia.Common.Exceptions;

namespace Fenicia.Module.SocialNetwork.Domains.User;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<UserResponse> AddAsync(UserRequest request, CancellationToken cancellationToken)
    {
        var user = UserConverter.Convert(request);

        userRepository.Add(user);

        await userRepository.SaveChangesAsync(cancellationToken);

        return UserConverter.Convert(user);
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);

        return user is null ? null : UserConverter.Convert(user);
    }

    public async Task<UserResponse?> UpdateAsync(Guid id, UserRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);
        }

        var model = UserConverter.Convert(request);

        userRepository.Update(model);

        await userRepository.SaveChangesAsync(cancellationToken);

        return UserConverter.Convert(model);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        userRepository.Delete(id);

        await userRepository.SaveChangesAsync(cancellationToken);
    }
}
