using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.DTOs.Basic.StockMovement;

public class MovementFormData
{
    public MovementFormData()
    {
    }

    public MovementFormData(
        Guid? id,
        Guid productId,
        StockMovementType type,
        double quantity,
        decimal price,
        DateTime? date,
        Guid? customerId,
        Guid? supplierId,
        Guid? employeeId,
        string? reason)
    {
        Id = id;
        ProductId = productId;
        Type = type;
        Quantity = quantity;
        Price = price;
        Date = date;
        CustomerId = customerId;
        SupplierId = supplierId;
        EmployeeId = employeeId;
        Reason = reason;
    }

    public Guid? Id { get; set; }

    public Guid ProductId { get; set; }

    public StockMovementType Type { get; set; }

    public double Quantity { get; set; }

    public decimal Price { get; set; }

    public DateTime? Date { get; set; }

    public Guid? CustomerId { get; set; }

    public Guid? SupplierId { get; set; }

    public Guid? EmployeeId { get; set; }

    public string? Reason { get; set; }
}
