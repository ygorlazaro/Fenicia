using Fenicia.Web.Components.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class InventoryKpiCards
{
    [Parameter]
[EditorRequired]
public DashboardData Dashboard { get; set; } = default!;

    [Parameter]
    public Color ProfitColor { get; set; } = Color.Success;
}
