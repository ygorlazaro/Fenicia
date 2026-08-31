using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Position.DTOs;

public record DeletePositionCommand([Required] Guid Id);
