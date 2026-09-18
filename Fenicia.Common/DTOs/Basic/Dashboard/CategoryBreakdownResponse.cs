namespace Fenicia.Common.DTOs.Basic.Dashboard;

public class CategoryBreakdownResponse
{
    public string Category { get; set; } = string.Empty;

    public decimal Revenue { get; set; }

    public double Quantity { get; set; }

    public bool IsOther { get; set; }
}
