using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class InventoryCharts
{
    [Parameter]
[EditorRequired]
public List<ChartSeries<double>> Series { get; set; } = [];

    [Parameter]
[EditorRequired]
public string[] Labels { get; set; } = [];

    [Parameter]
[EditorRequired]
public BarChartOptions Options { get; set; } = new();
}
