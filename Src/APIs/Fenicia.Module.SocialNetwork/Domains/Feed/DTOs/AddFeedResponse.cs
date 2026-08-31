namespace Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;

public record AddFeedResponse(Guid Id, DateTime Date, string Text, Guid UserId, Guid CompanyId);
