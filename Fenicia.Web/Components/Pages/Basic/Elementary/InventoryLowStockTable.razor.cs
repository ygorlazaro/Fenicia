using Fenicia.Web.Components.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class InventoryLowStockTable
{
    [Parameter]
[EditorRequired]
public List<LowStockItem> Items { get; set; } = [];
}
