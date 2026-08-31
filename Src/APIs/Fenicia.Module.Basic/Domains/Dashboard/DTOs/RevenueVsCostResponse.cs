using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Dashboard.DTOs;

public record RevenueVsCostResponse(

    [Required][MaxLength(200)] string Period,

    [Required] DateTime Date,

    decimal Revenue,

    decimal Cost,

    decimal Profit);
