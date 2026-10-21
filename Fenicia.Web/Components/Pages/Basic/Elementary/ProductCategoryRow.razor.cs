using Fenicia.Common.DTOs.Basic.ProductCategory;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class ProductCategoryRow
{
    [Parameter]
    public ProductCategoryResponse Item { get; set; } = null!;

    [Parameter]
    public Fenicia.Web.Components.Shared.CrudPage<ProductCategoryResponse> Page { get; set; } = null!;
}
