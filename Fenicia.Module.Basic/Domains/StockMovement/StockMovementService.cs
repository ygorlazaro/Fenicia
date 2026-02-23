using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Exceptions;
using Fenicia.Module.Basic.Domains.Product;

namespace Fenicia.Module.Basic.Domains.StockMovement;

public class StockMovementService(
    IStockMovementRepository stockMovementRepository,
    IProductRepository productRepository) : IStockMovementService
{
    public async Task AddStock(Guid productId, int quantity, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(productId, ct)
                      ?? throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);
        product.Quantity += quantity;

        productRepository.Update(product);

        var movement = new StockMovementModel
        {
            Quantity = quantity,
            ProductId = productId,
            Date = DateTime.UtcNow,
            Price = product.CostPrice,
            Type = StockMovementType.In
        };

        stockMovementRepository.Add(movement);

        await stockMovementRepository.SaveChangesAsync(ct);
    }

    public async Task RemoveStock(Guid productId, int quantity, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(productId, ct)
                      ?? throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);
        product.Quantity -= quantity;

        productRepository.Update(product);

        var movement = new StockMovementModel
        {
            Quantity = quantity,
            ProductId = productId,
            Date = DateTime.UtcNow,
            Price = product.SalesPrice,
            Type = StockMovementType.Out
        };

        stockMovementRepository.Add(movement);

        await stockMovementRepository.SaveChangesAsync(ct);
    }

    public async Task<List<StockMovementResponse>> GetMovementAsync(
        Guid productId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct,
        int page = 1,
        int perPage = 10)
    {
        var movements =
            await stockMovementRepository.GetMovementAsync(productId, startDate, endDate, ct, page, perPage);

        return [..movements.Select(m => new StockMovementResponse(m))];
    }

    public async Task<List<StockMovementResponse>> GetMovementAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct,
        int page = 1,
        int perPage = 10)
    {
        var movements = await stockMovementRepository.GetMovementAsync(startDate, endDate, ct, page, perPage);

        return [..movements.Select(m => new StockMovementResponse(m))];
    }

    public async Task<StockMovementResponse?> AddAsync(StockMovementRequest request, CancellationToken ct)
    {
        var stockMovement = new StockMovementModel(request);

        stockMovementRepository.Add(stockMovement);

        if (stockMovement.Type == StockMovementType.In)
            await productRepository.IncreaseStockAsync(stockMovement.ProductId, stockMovement.Quantity, ct);
        else if (stockMovement.Type == StockMovementType.In)
            await productRepository.DecreastStockAsync(stockMovement.ProductId, stockMovement.Quantity, ct);

        await stockMovementRepository.SaveChangesAsync(ct);

        return new StockMovementResponse(stockMovement);
    }

    public async Task<StockMovementResponse?> UpdateAsync(Guid id, StockMovementRequest request, CancellationToken ct)
    {
        var stockMovement = await stockMovementRepository.GetByIdAsync(id, ct);

        if (stockMovement is null) return null;

        stockMovement.Date = request.Date;
        stockMovement.Type = request.Type;
        stockMovement.ProductId = request.ProductId;
        stockMovement.CustomerId = request.CustomerId;
        stockMovement.Quantity = request.Quantity;
        stockMovement.Price = request.Price;
        stockMovement.SupplierId = request.SupplierId;

        stockMovementRepository.Update(stockMovement);

        await stockMovementRepository.SaveChangesAsync(ct);

        return new StockMovementResponse(stockMovement);
    }
}