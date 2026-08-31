namespace Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;

public record GetFollowersResponse(Guid Id, Guid UserId, DateTime FollowDate);
