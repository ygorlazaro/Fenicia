using Fenicia.Web.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class InventoryLowStockTable
{
    [Parameter]
[EditorRequired]
public List<LowStockItem> Items { get; set; } = [];
}
