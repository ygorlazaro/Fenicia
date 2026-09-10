namespace Fenicia.Web.Components.Pages.Basic.Models;

using Fenicia.Web.Services;

public class OrderDto : IEquatable<OrderDto>, ICrudItem
{
    public Guid Id { get; init; }

    public string OrderNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public Guid? EmployeeId { get; set; }

    public string? EmployeeName { get; set; }

    public decimal TotalAmount { get; set; }

    public int TotalQuantity { get; set; }

    public int TotalItems { get; set; }

    public DateTime SaleDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public bool Equals(OrderDto? other)
    {
        if (other is null)
        {
            return false;
        }

        return Id == other.Id && OrderNumber == other.OrderNumber;
    }

    public override bool Equals(object? obj) => Equals(obj as OrderDto);

    public override int GetHashCode() => HashCode.Combine(Id, OrderNumber);
}
