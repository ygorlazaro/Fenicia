using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Position.DTOs;

public record GetAllPositionResponse([Required] Guid Id, [Required] [MaxLength(200)] string Name);