using Fenicia.Web.Components.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class InventoryZeroMovementTable
{
    [Parameter]
[EditorRequired]
public List<ZeroMovementItem> Items { get; set; } = [];
}
