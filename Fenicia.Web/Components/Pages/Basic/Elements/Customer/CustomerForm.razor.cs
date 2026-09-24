using Fenicia.Common.DTOs.Auth.State;
using Fenicia.Web.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elements.Customer;

public partial class CustomerForm : ComponentBase
{
    [Parameter]
    public CustomerFormModel Model { get; set; } = new();

    [Parameter]
    public IReadOnlyList<StateResponse>? States { get; set; }
}