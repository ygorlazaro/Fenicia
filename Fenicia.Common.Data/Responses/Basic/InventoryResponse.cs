using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Common.Data.Responses.Basic;

public class InventoryResponse
{
    public decimal TotalCostPrice { get; set; }

    public decimal TotalSalesPrice { get; set; }

    public decimal TotalProfit => this.TotalSalesPrice - this.TotalCostPrice;

    public double TotalQuantity { get; set; }

    public List<InventoryDetailResponse> Items { get; set; }

    public static InventoryResponse Map(List<ProductModel> models)
    {
        var inventory = new InventoryResponse
        {
            Items =
            [
                .. models.Select(p => new InventoryDetailResponse
                {
                    ProductId = p.Id,
                    Quantity = p.Quantity,
                    CategoryId = p.CategoryId,
                    CostPrice = p.CostPrice,
                    ProductName = p.Name,
                    CategoryName = p.Category.Name,
                    SalesPrice = p.SalesPrice
                })
            ]
        };

        return inventory;
    }
}