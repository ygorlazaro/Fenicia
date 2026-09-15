using Fenicia.Common.DTOs.Basic.Supplier;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class SupplierRow
{
    [Parameter]
    public GetAllSupplierResponse Item { get; set; } = default!;

    [Parameter]
    public EventCallback<GetAllSupplierResponse> OnEdit { get; set; }

    [Parameter]
    public EventCallback<GetAllSupplierResponse> OnDelete { get; set; }
}
