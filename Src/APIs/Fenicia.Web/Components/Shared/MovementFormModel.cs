namespace Fenicia.Web.Components.Shared;

using Fenicia.Common.Enums.Basic;

public class MovementFormModel
{
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