using Fenicia.Common.DTOs.Basic.Customer;
using Fenicia.Common.DTOs.Basic.State;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elements.Customer;

public partial class CustomerForm : ComponentBase
{
    [Parameter]
    public CustomerFormData Model { get; set; } = new();

    [Parameter]
    public IReadOnlyList<GetAllStateResponse>? States { get; set; }
}
