using Fenicia.Common.DTOs.Basic.Order;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class OrderDetailItemsTable
{
    [Parameter]
[EditorRequired]
public List<OrderDetailResponse> Items { get; set; } = [];
}
