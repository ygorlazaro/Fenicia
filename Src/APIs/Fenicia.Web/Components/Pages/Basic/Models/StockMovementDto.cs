namespace Fenicia.Web.Components.Pages.Basic.Models;

using Fenicia.Common.Enums.Basic;

public class StockMovementDto
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public double Quantity { get; set; }

    public DateTime? Date { get; set; }

    public decimal? Price { get; set; }

    public StockMovementType Type { get; set; }

    public Guid? CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public Guid? SupplierId { get; set; }

    public string SupplierName { get; set; } = string.Empty;

    public Guid? EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public Guid? OrderId { get; set; }

    public string? Reason { get; set; }
}