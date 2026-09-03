using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Block.DTOs;

public record UnblockCommand([Required] Guid BlockedUserId);