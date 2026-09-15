using Fenicia.Common.DTOs.Basic.Product;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class ProductRow
{
    [Parameter]
    public GetAllProductResponse Item { get; set; } = default!;

    [Parameter]
    public EventCallback<GetAllProductResponse> OnEdit { get; set; }

    [Parameter]
    public EventCallback<GetAllProductResponse> OnDelete { get; set; }
}
