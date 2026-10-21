using Fenicia.Common.DTOs.Basic.Product;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class ProductRow
{
    [Parameter]
    public ProductResponse Item { get; set; } = null!;

    [Parameter]
    public EventCallback<ProductResponse> OnEdit { get; set; }

    [Parameter]
    public EventCallback<ProductResponse> OnDelete { get; set; }
}
