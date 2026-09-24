using Fenicia.Common.DTOs.Basic.Customer;
using Microsoft.AspNetCore.Components;

namespace Fenicia.Web.Components.Pages.Basic.Elements.Customer;

public partial class CustomerRow : ComponentBase
{
    [Parameter]

    public GetAllCustomerResponse Item { get; set; } = null!;

    [Parameter]

    public EventCallback<GetAllCustomerResponse> OnEdit { get; set; }

    [Parameter]

    public EventCallback<GetAllCustomerResponse> OnDelete { get; set; }
}
