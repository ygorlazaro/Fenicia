namespace Fenicia.Module.SocialNetwork.Domains.Profile.DTOs;

public record UpdateProfileCommand(Guid Id, string? Bio, string? ImageUrl, string? Website, string? Location, string? Phone, DateTime? BirthDate);
