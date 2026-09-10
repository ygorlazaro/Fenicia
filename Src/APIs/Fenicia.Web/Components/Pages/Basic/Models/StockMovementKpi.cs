namespace Fenicia.Web.Components.Pages.Basic.Models;

public class StockMovementKpi
{
    public double TotalInQuantity { get; set; }

    public double TotalOutQuantity { get; set; }

    public int TotalInEvents { get; set; }

    public int TotalOutEvents { get; set; }

    public decimal TotalInValue { get; set; }

    public decimal TotalOutValue { get; set; }
}