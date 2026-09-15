using Fenicia.Web.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class SupplierForm
{
    [Parameter]
    public SupplierFormModel Model { get; set; } = new();

    [Parameter]
    public IReadOnlyList<StateOption> States { get; set; } = [];

}
