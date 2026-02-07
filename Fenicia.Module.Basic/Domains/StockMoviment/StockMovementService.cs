using Fenicia.Common;
using Fenicia.Common.Data.Mappers.Basic;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Exceptions;
using Fenicia.Module.Basic.Domains.Product;

namespace Fenicia.Module.Basic.Domains.StockMoviment;

public class StockMovementService(IStockMovementRepository stockMovementRepository, IProductRepository productRepository) : IStockMovementService
{
    public async Task AddStock(Guid productId, int quantity, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(productId, ct);

        if (product is null)
        {
            throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);
        }

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
        var product = await productRepository.GetByIdAsync(productId, ct);

        if (product is null)
        {
            throw new ItemNotExistsException(TextConstants.ItemNotFoundMessage);
        }

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

    public async Task<List<StockMovementResponse>> GetMovementAsync(Guid productId, DateTime startDate, DateTime endDate, CancellationToken ct, int page = 1, int perPage = 10)
    {
        var movements = await stockMovementRepository.GetMovementAsync(productId, startDate, endDate, ct, page, perPage);

        return StockMovementMapper.Map(movements);
    }

    public async Task<List<StockMovementResponse>> GetMovementAsync(DateTime startDate, DateTime endDate, CancellationToken ct, int page = 1, int perPage = 10)
    {
        var movements = await stockMovementRepository.GetMovementAsync(startDate, endDate, ct, page, perPage);

        return StockMovementMapper.Map(movements);
    }

    public async Task<StockMovementResponse?> AddAsync(StockMovementRequest request, CancellationToken ct)
    {
        var stockMovement = StockMovementMapper.Map(request);

        stockMovementRepository.Add(stockMovement);

        if (stockMovement.Type == StockMovementType.In)
        {
            await productRepository.IncreaseStockAsync(stockMovement.ProductId, stockMovement.Quantity, ct);
        }
        else if (stockMovement.Type == StockMovementType.In)
        {
            await productRepository.DecreastStockAsync(stockMovement.ProductId, stockMovement.Quantity, ct);
        }

        await stockMovementRepository.SaveChangesAsync(ct);

        return StockMovementMapper.Map(stockMovement);
    }

    public async Task<StockMovementResponse?> UpdateAsync(Guid id, StockMovementRequest request, CancellationToken ct)
    {
        var moviment = await stockMovementRepository.GetByIdAsync(id, ct);

        if (moviment is null)
        {
            return null;
        }

        moviment.Date = request.Date;
        moviment.Type = request.Type;
        moviment.ProductId = request.ProductId;
        moviment.CustomerId = request.CustomerId;
        moviment.Quantity = request.Quantity;
        moviment.Price = request.Price;
        moviment.SupplierId = request.SupplierId;

        stockMovementRepository.Update(moviment);

        await stockMovementRepository.SaveChangesAsync(ct);

        return StockMovementMapper.Map(moviment);
    }
}
