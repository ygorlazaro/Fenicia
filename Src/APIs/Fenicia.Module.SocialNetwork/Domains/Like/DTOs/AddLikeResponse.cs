namespace Fenicia.Module.SocialNetwork.Domains.Like.DTOs;

public record AddLikeResponse(Guid Id, Guid UserId, Guid FeedId, DateTime LikeDate, Guid CompanyId);
