using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Position;

public record GetPositionByIdQuery([Required] Guid Id);