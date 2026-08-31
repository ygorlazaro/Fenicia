namespace Fenicia.Module.SocialNetwork.Domains.Share.DTOs;

public record ShareCommand(Guid Id, Guid OriginalFeedId, string? Text);
