using Fenicia.Common.DTOs.Basic.Customer;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elements.Customer;

public partial class CustomerRow : ComponentBase
{
    [Parameter]
    public CustomerResponse Item { get; set; } = null!;

    [Parameter]
    public EventCallback<CustomerResponse> OnEdit { get; set; }

    [Parameter]
    public EventCallback<CustomerResponse> OnDelete { get; set; }
}