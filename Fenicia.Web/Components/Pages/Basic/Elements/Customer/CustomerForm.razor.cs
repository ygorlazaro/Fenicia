using Fenicia.Common.DTOs.Auth.State;
using Fenicia.Common.DTOs.Basic.Customer;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elements.Customer;

public partial class CustomerForm : ComponentBase
{
    [Parameter]

    public CustomerFormData Model { get; set; } = new();

    [Parameter]

    public IReadOnlyList<StateResponse>? States { get; set; }
}
