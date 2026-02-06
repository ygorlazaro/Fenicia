using Fenicia.Common.Data.Converters.SocialNetwork;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Responses.SocialNetwork;

namespace Fenicia.Module.SocialNetwork.Domains.Follower;

public class FollowerService(IFollowerRepository followerRepository) : IFollowerService
{
    public async Task<FollowerResponse?> FollowAsync(Guid userId, Guid followerId, CancellationToken cancellationToken)
    {
        var follower = new FollowerModel()
        {
            UserId = userId,
            FollowerId = followerId,
            FollowDate = DateTime.UtcNow,
            IsActive = true
        };

        followerRepository.Add(follower);

        await followerRepository.SaveChangesAsync(cancellationToken);

        return FollowerConverter.Convert(follower);
    }

    public async Task<FollowerResponse?> UnfollowAsync(Guid userId, Guid followerId, CancellationToken cancellationToken)
    {
        var follower = await followerRepository.FindFollowerAsync(userId, followerId, cancellationToken);

        if (follower is null)
        {
            return null;
        }

        follower.IsActive = false;

        followerRepository.Update(follower);

        await followerRepository.SaveChangesAsync(cancellationToken);

        return FollowerConverter.Convert(follower);
    }

    public async Task<List<FollowerResponse>> GetFollowersAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        var followers = await followerRepository.GetFollowersAsync(userId, cancellationToken, page, perPage);

        return FollowerConverter.Convert(followers);
    }

    public async Task<int> CountAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await followerRepository.CountAsync(x => x.UserId == userId, cancellationToken);
    }
}
