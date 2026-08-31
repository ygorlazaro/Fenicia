namespace Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;

public record GetFeedByIdResponse(Guid Id, DateTime Date, string Text, Guid UserId, Guid CompanyId, int CommentsCount, int LikesCount, int SharesCount);
