using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.Data.Models;

[Table("stock_movements", Schema = "basic")]
public class BasicStockMovementModel : BaseCompanyModel
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public double Quantity { get; set; }

    public DateTime? Date { get; set; }

    [Required]
    public decimal? Price { get; set; }

    [Required]
    public StockMovementType Type { get; set; }

    [MaxLength(255)]
    public string? Reason { get; set; }

    [ForeignKey(nameof(ProductId))]
    public BasicProductModel Product { get; set; } = null!;

    public Guid? CustomerId { get; set; }

    public Guid? SupplierId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public BasicCustomerModel? Customer { get; set; }

    [ForeignKey(nameof(SupplierId))]
    public BasicSupplierModel? Supplier { get; set; }

    public Guid? EmployeeId { get; set; }

    [ForeignKey(nameof(EmployeeId))]
    public BasicEmployeeModel? Employee { get; set; }

    public Guid? OrderId { get; set; }

    [ForeignKey(nameof(OrderId))]
    public BasicOrderModel? OrderModel { get; set; }
}
