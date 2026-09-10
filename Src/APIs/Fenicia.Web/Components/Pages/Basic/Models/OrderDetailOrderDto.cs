namespace Fenicia.Web.Components.Pages.Basic.Models;

using Fenicia.Common.Enums.Basic;

public class OrderDetailOrderDto
{
    public Guid Id { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public int TotalQuantity { get; set; }

    public DateTime SaleDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public PaymentMethod PaymentMethod { get; set; }

    public string? Notes { get; set; }

    public Guid? EmployeeId { get; set; }

    public string? EmployeeName { get; set; }
}
