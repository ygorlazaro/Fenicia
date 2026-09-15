using Fenicia.Common.DTOs.Basic.Order;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class OrderRow
{
    [Parameter]
    public GetAllOrderResponse Item { get; set; } = default!;
}
