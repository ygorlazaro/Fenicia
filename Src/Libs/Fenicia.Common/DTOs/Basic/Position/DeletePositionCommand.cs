using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Position;

public record DeletePositionCommand([Required] Guid Id);