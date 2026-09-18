using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.StockMovement;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.StockMovement;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class StockMovementMapper
{
    [MapProperty("Product.Name", nameof(GetStockMovementResponse.ProductName))]
    [MapProperty("Customer.Person.Name", nameof(GetStockMovementResponse.CustomerName))]
    [MapProperty("Supplier.Person.Name", nameof(GetStockMovementResponse.SupplierName))]
    [MapProperty("Employee.Person.Name", nameof(GetStockMovementResponse.EmployeeName))]
    public partial GetStockMovementResponse MapToGetStockMovementResponse(StockMovementModel movement);

    public partial AddStockMovementResponse MapToAddStockMovementResponse(StockMovementModel movement);

    public partial UpdateStockMovementResponse MapToUpdateStockMovementResponse(StockMovementModel movement);

    [MapProperty("Product.Name", nameof(StockMovementHistoryResponse.ProductName))]
    [MapProperty("Customer.Person.Name", nameof(StockMovementHistoryResponse.CustomerName))]
    [MapProperty("Supplier.Person.Name", nameof(StockMovementHistoryResponse.SupplierName))]
    public partial StockMovementHistoryResponse MapToStockMovementHistoryResponse(StockMovementModel movement);
}
