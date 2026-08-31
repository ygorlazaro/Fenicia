namespace Fenicia.Module.SocialNetwork.Domains.Like.DTOs;

public record GetLikesResponse(Guid Id, Guid UserId, Guid FeedId, DateTime LikeDate);
