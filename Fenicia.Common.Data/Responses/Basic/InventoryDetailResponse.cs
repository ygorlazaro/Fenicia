using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Common.Data.Responses.Basic;

public class InventoryDetailResponse(ProductModel p)
{
    public Guid ProductId { get; set; } = p.Id;

    public double Quantity { get; set; } = p.Quantity;

    public decimal? CostPrice { get; set; } = p.CostPrice;

    public decimal SalesPrice { get; set; } = p.SalesPrice;

    public decimal Profit => this.SalesPrice - this.CostPrice ?? 0;

    public string ProductName { get; set; } = p.Name;

    public Guid CategoryId { get; set; } = p.CategoryId;

    public string CategoryName { get; set; } = p.Category.Name;
}