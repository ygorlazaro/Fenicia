namespace Fenicia.Module.SocialNetwork.Domains.Like.DTOs;

public record GetLikesByFeedQuery(int Page = 1, int PerPage = 10, Guid FeedId = default);
