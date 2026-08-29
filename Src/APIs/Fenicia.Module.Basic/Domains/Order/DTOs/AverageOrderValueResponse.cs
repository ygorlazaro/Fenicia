namespace Fenicia.Module.Basic.Domains.Order.DTOs;

public record AverageOrderValueResponse
{
    public decimal AverageValue { get; set; }

    public int TotalOrders { get; set; }

    public decimal MedianValue { get; set; }

    public decimal MinValue { get; set; }

    public decimal MaxValue { get; set; }
}
