using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class StockMovementHeader
{
    [Parameter]
    public EventCallback OnNewMovement { get; set; }
}
