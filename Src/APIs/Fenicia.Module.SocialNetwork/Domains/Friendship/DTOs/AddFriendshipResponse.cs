namespace Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;

public record AddFriendshipResponse(Guid Id, Guid UserId, Guid TargetUserId, DateTime FollowDate, bool IsActive);
