using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Responses.SocialNetwork;

namespace Fenicia.Module.SocialNetwork.Domains.Follower;

public class FollowerService(IFollowerRepository followerRepository) : IFollowerService
{
    public async Task<FollowerResponse?> FollowAsync(Guid userId, Guid followerId, CancellationToken ct)
    {
        var follower = new FollowerModel
        {
            UserId = userId,
            FollowerId = followerId,
            FollowDate = DateTime.UtcNow,
            IsActive = true
        };

        followerRepository.Add(follower);

        await followerRepository.SaveChangesAsync(ct);

        return new FollowerResponse(follower);
    }

    public async Task<FollowerResponse?> UnfollowAsync(Guid userId, Guid followerId, CancellationToken ct)
    {
        var follower = await followerRepository.FindFollowerAsync(userId, followerId, ct);

        if (follower is null) return null;

        follower.IsActive = false;

        followerRepository.Update(follower);

        await followerRepository.SaveChangesAsync(ct);

        return new FollowerResponse(follower);
    }

    public async Task<List<FollowerResponse>> GetFollowersAsync(
        Guid userId,
        CancellationToken ct,
        int page = 1,
        int perPage = 10)
    {
        var followers = await followerRepository.GetFollowersAsync(userId, ct, page, perPage);

        return [..followers.Select(f => new FollowerResponse(f))];
    }

    public async Task<int> CountAsync(Guid userId, CancellationToken ct)
    {
        return await followerRepository.CountAsync(x => x.UserId == userId, ct);
    }
}