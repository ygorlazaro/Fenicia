using Fenicia.Common.DTOs.Basic.Product;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class OrderFormProductSearch
{
    [Parameter]
[EditorRequired]
public IEnumerable<GetAllProductResponse> Products { get; set; } = [];

    [Parameter]
    public string Search { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<string?> SearchChanged { get; set; }

    [Parameter]
    public bool Loading { get; set; }

    [Parameter]
    public EventCallback<KeyboardEventArgs> OnSearchKeyDown { get; set; }

    [Parameter]
    public EventCallback<GetAllProductResponse> OnProductClick { get; set; }

    private IEnumerable<GetAllProductResponse> FilteredProducts
    {
        get
        {
            var all = Products ?? [];
            var active = all.Where(p => p.IsActive).ToList();
            return active.OrderBy(p => p.Name);
        }
    }
}
