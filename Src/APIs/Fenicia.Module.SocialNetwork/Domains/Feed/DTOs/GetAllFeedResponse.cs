namespace Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;

public record GetAllFeedResponse(Guid Id, DateTime Date, string Text, Guid UserId, Guid CompanyId, int CommentsCount, int LikesCount, int SharesCount);
