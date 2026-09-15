using Fenicia.Web.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class ProductForm
{
    [Parameter]
    public ProductFormModel Model { get; set; } = new();

    [Parameter]
    public List<CategoryOption> Categories { get; set; } = [];

    [Parameter]
    public List<SupplierOption> Suppliers { get; set; } = [];

}
