using Fenicia.Common.DTOs.Basic.Customer;
using Fenicia.Common.DTOs.Basic.Employee;
using Fenicia.Common.DTOs.Basic.Order;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Web.Components.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class OrderFormCart
{
    [Parameter]
[EditorRequired]
public List<OrderFormCartItem> Cart { get; set; } = [];

    [Parameter]
[EditorRequired]
public Guid CartCustomerId { get; set; }

    [Parameter]
    public Guid? CartEmployeeId { get; set; }

    [Parameter]
    public PaymentMethod CartPaymentMethod { get; set; } = PaymentMethod.Cash;

    [Parameter]
    public OrderStatus CartStatus { get; set; } = OrderStatus.Approved;

    [Parameter]
    public decimal CartDiscount { get; set; }

    [Parameter]
    public string? CartNotes { get; set; }

    [Parameter]
[EditorRequired]
public List<OrderFormCustomerOption> Customers { get; set; } = [];

    [Parameter]
[EditorRequired]
public List<OrderFormEmployeeOption> Employees { get; set; } = [];

    [Parameter]
[EditorRequired]
public List<GetAllOrderResponse>? RecentOrders { get; set; }

    [Parameter]
    public bool IsSubmitting { get; set; }

    [Parameter]
    public EventCallback<Guid> OnRemove { get; set; }

    [Parameter]
    public EventCallback<OrderFormCartUpdatePayload> OnUpdateQty { get; set; }

    [Parameter]
    public EventCallback<Guid> OnIncrease { get; set; }

    [Parameter]
    public EventCallback<Guid> OnDecrease { get; set; }

    [Parameter]
    public EventCallback OnFinalize { get; set; }

    private int TotalQuantity => (int)Cart.Sum(c => c.Quantity);

    private decimal Subtotal => Cart.Sum(c => c.Subtotal);

    private decimal Total => Math.Max(0, Subtotal - CartDiscount);

    private bool CanFinalize => Cart.Any() && CartCustomerId != Guid.Empty;
}
