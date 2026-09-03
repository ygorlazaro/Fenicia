using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Position.DTOs;

public record GetPositionByIdResponse([Required] Guid Id, [Required] [MaxLength(200)] string Name);