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
    public string? Search { get; set; }

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
            var query = Products.Where(p => p.IsActive);

            if (string.IsNullOrWhiteSpace(Search))
            {
                return [.. query.OrderBy(p => p.Name)];
            }

            {
                var term = Search.Trim();

                query = query.Where(p =>

                    p.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || (p.SKU ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase)
                    || (p.Barcode ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase));
            }
            return [.. query.OrderBy(p => p.Name)];
        }
    }
}
