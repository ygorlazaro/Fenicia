using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.Data.Models.Basic;

[Table("stock_movements")]
public sealed class StockMovementModel : BaseModel
{
    public StockMovementModel(StockMovementRequest request)
    {
        this.Id =  request.Id;
        this.ProductId = request.ProductId;
        this.Quantity = request.Quantity;
        this.Date = request.Date;
        this.Price = request.Price;
        this.Type = request.Type;
        this.CustomerId = request.CustomerId;
        this.SupplierId =  request.SupplierId;
    }

    public StockMovementModel()
    {
        
    }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    public double Quantity { get; set; }

    public DateTime? Date { get; set; }

    [Required]
    public decimal? Price { get; set; }

    [Required]
    public StockMovementType Type { get; set; }

    [ForeignKey(nameof(ProductId))]
    public ProductModel Product { get; set; } = null!;

    public Guid? CustomerId { get; set; }

    public Guid? SupplierId { get; set; }

    [ForeignKey(nameof(CustomerId))]
    public CustomerModel? Customer { get; set; }

    [ForeignKey(nameof(SupplierId))]
    public SupplierModel? Supplier { get; set; }
}