using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Profile.DTOs;

public record GetProfileByIdQuery(
    [Required] Guid Id);
