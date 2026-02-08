using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Responses.Basic;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Common.Data.Responses.Auth;

public class StockMovementResponse(StockMovementModel model)
{
    public Guid Id { get; set; } = model.Id;

    public double Quantity { get; set; } = model.Quantity;

    public DateTime? Date { get; set; } = model.Date;

    public decimal? Price { get; set; } = model.Price;

    public StockMovementType Type { get; set; } = model.Type;

    public ProductResponse Product { get; set; } = new (model.Product);

    public Guid? CustomerId { get; set; } = model.CustomerId;

    public Guid? SupplerId { get; set; }

    public Guid ProductId { get; set; } = model.ProductId;

    public Guid? SupplierId { get; set; } = model.SupplierId;
}