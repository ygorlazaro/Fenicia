namespace Fenicia.Module.Basic.Domains.Dashboard.DTOs;

public record CategoryBreakdownResponse
{
    public string Category { get; set; } = string.Empty;

    public decimal Revenue { get; set; }

    public double Quantity { get; set; }

    public bool IsOther { get; set; }
}
