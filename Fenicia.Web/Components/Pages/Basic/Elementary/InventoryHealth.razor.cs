using Fenicia.Web.Components.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class InventoryHealth
{
    [Parameter]
    public HealthData? Health { get; set; }

    [Parameter]
    public int ZeroMovementDays { get; set; } = 90;

    [Parameter]
    public List<ChartSeries<double>> StockSeries { get; set; } = [];

    [Parameter]
    public string[] StockLabels { get; set; } = [];

    [Parameter]
    public BarChartOptions StockOptions { get; set; } = new();
}
