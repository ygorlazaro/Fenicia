namespace Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;

public record UpdateFeedCommand(Guid Id, DateTime Date, string Text);
