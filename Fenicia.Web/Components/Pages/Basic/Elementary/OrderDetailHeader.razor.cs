using Fenicia.Common.DTOs.Basic.Order;
using Fenicia.Common.Enums.Auth;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class OrderDetailHeader
{
    [Parameter]
[EditorRequired]
public GetOrderByIdResponse Order { get; set; } = default!;

    [Parameter]
    public EventCallback OnPrint { get; set; }

    private static Color StatusColor(string status)
    {
        return status switch
        {
            "Approved" => Color.Success,
            "Pending" => Color.Warning,
            "Cancelled" => Color.Error,

            _ => Color.Default
        };
    }
}
