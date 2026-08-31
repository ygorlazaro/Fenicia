namespace Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;

public record AddFeedCommand(Guid Id, DateTime Date, string Text, Guid UserId);
