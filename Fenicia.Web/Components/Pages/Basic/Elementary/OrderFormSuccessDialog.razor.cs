using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class OrderFormSuccessDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public string? LastOrderNumber { get; set; }

    [Parameter]
    public decimal LastOrderTotal { get; set; }

    [Parameter]
    public Guid LastOrderId { get; set; }

    [Parameter]
    public DialogOptions DialogOptions { get; set; } = new() { FullWidth = true, MaxWidth = MaxWidth.Small };

    [Parameter]
    public EventCallback OnStartNewSale { get; set; }
}
