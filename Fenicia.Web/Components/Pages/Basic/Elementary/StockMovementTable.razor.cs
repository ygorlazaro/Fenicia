using Fenicia.Common.DTOs.Basic.StockMovement;
using Fenicia.Common.Enums.Basic;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Fenicia.Web.Components.Pages.Basic.Elementary;

public partial class StockMovementTable
{

    [Parameter]
    public List<GetStockMovementResponse> Items { get; set; } = [];

    [Parameter]
    public bool Loading { get; set; }

    [Parameter]
    public EventCallback<GetStockMovementResponse> OnEdit { get; set; }

    private static string ResolveOrigin(GetStockMovementResponse m)
    {
        if (!string.IsNullOrEmpty(m.SupplierName))
        {
            return $"Fornecedor: {m.SupplierName}";
        }

        if (!string.IsNullOrEmpty(m.CustomerName))
        {
            return $"Cliente: {m.CustomerName}";
        }

        if (!string.IsNullOrEmpty(m.EmployeeName))
        {
            return $"Funcionário: {m.EmployeeName}";
        }

        return "—";
    }
}
