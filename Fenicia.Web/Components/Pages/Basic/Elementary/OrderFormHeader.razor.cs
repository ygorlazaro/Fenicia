using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class OrderFormHeader
{
    [Parameter]
    public bool Loading { get; set; }

    [Parameter]
    public bool HasCartItems { get; set; }

    [Parameter]
    public EventCallback OnRefresh { get; set; }

    [Parameter]
    public EventCallback OnClearCart { get; set; }
}
