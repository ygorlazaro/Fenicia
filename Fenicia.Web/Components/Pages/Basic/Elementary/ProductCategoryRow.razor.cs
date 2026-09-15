using Fenicia.Common.DTOs.Basic.ProductCategory;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class ProductCategoryRow
{
    [Parameter]
    public GetAllProductCategoryResponse Item { get; set; } = default!;

    [Parameter]
    public Fenicia.Web.Components.Shared.CrudPage<GetAllProductCategoryResponse> Page { get; set; } = default!;
}
