using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Block;

public record BlockCommand([Required] Guid BlockedProfileId);
