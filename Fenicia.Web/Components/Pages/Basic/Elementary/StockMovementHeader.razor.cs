using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class StockMovementHeader
{
    [Parameter]
    public EventCallback OnNewMovement { get; set; }
}
