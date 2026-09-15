using Fenicia.Common.DTOs.Basic.Customer;
using Fenicia.Common.DTOs.Basic.Employee;
using Fenicia.Common.DTOs.Basic.Product;
using Fenicia.Common.DTOs.Basic.StockMovement;
using Fenicia.Common.DTOs.Basic.Supplier;
using Fenicia.Common.Enums.Basic;
using Fenicia.Web.Components.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class StockMovementModal
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public MovementFormModel Model { get; set; } = new();

    [Parameter]
    public bool IsEdit { get; set; }

    [Parameter]
    public bool IsSubmitting { get; set; }

    [Parameter]
    public DialogOptions DialogOptions { get; set; } = new() { CloseButton = true, BackdropClick = false, MaxWidth = MaxWidth.Medium, FullWidth = true };

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public EventCallback OnSubmit { get; set; }

    [Parameter]
    public List<DatasourceItem> Products { get; set; } = [];

    [Parameter]
    public List<DatasourceItem> Suppliers { get; set; } = [];

    [Parameter]
    public List<DatasourceItem> Customers { get; set; } = [];

    [Parameter]
    public List<DatasourceItem> Employees { get; set; } = [];
}
