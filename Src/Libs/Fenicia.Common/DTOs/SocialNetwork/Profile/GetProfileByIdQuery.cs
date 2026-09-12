using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Profile;

public record GetProfileByIdQuery([Required] Guid Id);