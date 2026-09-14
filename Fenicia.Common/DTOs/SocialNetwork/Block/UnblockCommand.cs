using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Block;

public record UnblockCommand([Required] Guid BlockedProfileId);
