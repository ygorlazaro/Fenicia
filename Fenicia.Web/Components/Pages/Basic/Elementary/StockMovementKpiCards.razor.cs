using Fenicia.Common.DTOs.Basic.StockMovement;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class StockMovementKpiCards
{
    [Parameter, EditorRequired]
    public StockMovementKpi Kpi { get; set; } = default!;
}
