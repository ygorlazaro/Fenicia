using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record GetOrderByIdQuery([Required] Guid Id);
