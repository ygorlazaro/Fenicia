using Fenicia.Common.DTOs.Basic.StockMovement;
using Fenicia.Web.Components.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class StockMovementKpiCards
{
    [Parameter, EditorRequired]
    public StockMovementKpi Kpi { get; set; } = default!;
}
