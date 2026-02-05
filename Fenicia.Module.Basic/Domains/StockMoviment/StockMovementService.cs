using Fenicia.Common;
using Fenicia.Common.Database.Converters.Basic;
using Fenicia.Common.Database.Models.Basic;
using Fenicia.Common.Database.Requests.Basic;
using Fenicia.Common.Database.Responses.Auth;
using Fenicia.Common.Enums;
using Fenicia.Common.Exceptions;
using Fenicia.Module.Basic.Domains.Product;

namespace Fenicia.Module.Basic.Domains.StockMoviment;

public class StockMovementService(IStockMovementRepository stockMovementRepository, IProductRepository productRepository) : IStockMovementService
{
    public async Task AddStock(Guid productId, int quantity, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);

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

        await stockMovementRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveStock(Guid productId, int quantity, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);

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
            Price = product.SellingPrice,
            Type = StockMovementType.Out
        };

        stockMovementRepository.Add(movement);

        await stockMovementRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<StockMovementResponse>> GetMovementAsync(Guid productId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        var movements = await stockMovementRepository.GetMovementAsync(productId, startDate, endDate, cancellationToken, page, perPage);

        return StockMovementConverter.Convert(movements);
    }

    public async Task<List<StockMovementResponse>> GetMovementAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        var movements = await stockMovementRepository.GetMovementAsync(startDate, endDate, cancellationToken, page, perPage);

        return StockMovementConverter.Convert(movements);
    }

    public async Task<StockMovementResponse?> AddAsync(StockMovementRequest request, CancellationToken cancellationToken)
    {
        var stockMovement = StockMovementConverter.Convert(request);

        stockMovementRepository.Add(stockMovement);

        if (stockMovement.Type == StockMovementType.In)
        {
            await productRepository.IncreaseStockAsync(stockMovement.ProductId, stockMovement.Quantity, cancellationToken);
        }
        else if (stockMovement.Type == StockMovementType.In)
        {
            await productRepository.DecreastStockAsync(stockMovement.ProductId, stockMovement.Quantity, cancellationToken);
        }

        await stockMovementRepository.SaveChangesAsync(cancellationToken);

        return StockMovementConverter.Convert(stockMovement);
    }

    public async Task<StockMovementResponse?> UpdateAsync(Guid id, StockMovementRequest request, CancellationToken cancellationToken)
    {
        var moviment = await stockMovementRepository.GetByIdAsync(id, cancellationToken);

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

        await stockMovementRepository.SaveChangesAsync(cancellationToken);

        return StockMovementConverter.Convert(moviment);
    }
}
