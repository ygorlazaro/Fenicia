using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Position;

public record GetPositionByIdResponse([Required] Guid Id, [Required][MaxLength(200)] string Name) : ICrudItem;