namespace Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;

public record GetFollowingResponse(Guid Id, Guid TargetUserId, DateTime FollowDate);
