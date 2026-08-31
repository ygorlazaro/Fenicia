namespace Fenicia.Module.SocialNetwork.Domains.Profile.DTOs;

public record GetProfileByIdResponse(Guid Id, Guid UserId, string? Bio, string? ImageUrl, string? Website, string? Location, string? Phone, DateTime? BirthDate);
