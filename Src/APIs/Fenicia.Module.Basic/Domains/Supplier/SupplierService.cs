using Fenicia.Common;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Fenicia.Module.Basic.Domains.Address.Interfaces;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.PersonAddress.Interfaces;
using Fenicia.Module.Basic.Domains.Product.Interfaces;
using Fenicia.Module.Basic.Domains.StockMovement.Interfaces;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Fenicia.Module.Basic.Domains.Supplier.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier;

public sealed class SupplierService(
    ISupplierRepository supplierRepository,
    IProductService productService,
    IStockMovementService stockMovementService,
    IAddressService addressService,
    IPersonAddressService personAddressService) : ISupplierService
{
    public SupplierService()
        : this(null!, null!, null!, null!, null!)
    {
    }

    public async Task<Pagination<List<GetAllSupplierResponse>>> GetAllAsync(
        GetAllSupplierQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = supplierRepository.Query()
            .Include(s => s.Person)
            .Include(s => s.Person.PersonAddresses)
            .ThenInclude(pa => pa.Address)
            .ThenInclude(a => a.State);

        var filteredQuery = baseQuery;

        var total = await filteredQuery.CountAsync(cancellationToken);

        var suppliers = await filteredQuery
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);

        var response = suppliers.Select(s => s.MapToGetAllSupplierResponse()).ToList();

        return new Pagination<List<GetAllSupplierResponse>>(response, total, query.Page, query.PerPage);
    }

    public async Task<List<GetAllSupplierForDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default)
    {
        var suppliers = await supplierRepository.GetAllWithDetailsAsync(cancellationToken: cancellationToken);

        return [.. suppliers.Select(s => new GetAllSupplierForDataSourceResponse(s.Id, s.Person.Name))];
    }

    public async Task<GetSupplierByIdResponse?> GetByIdAsync(
        GetSupplierByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var supplier = await supplierRepository.GetByIdWithDetailsAsync(query.Id, cancellationToken);

        return supplier?.MapToGetSupplierByIdResponse();
    }

    public async Task<AddSupplierResponse> AddAsync(
        AddSupplierCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            Document = command.Document,
            PhoneNumber = command.PhoneNumber,
            CompanyId = companyId
        };

        Guid? addressId = null;

        if (command.Address != null)
        {
            var addressCommand = new AddressCommand(
                command.Address.Street,
                command.Address.Number,
                command.Address.Complement,
                command.Address.Neighborhood,
                command.Address.ZipCode,
                command.Address.StateId,
                command.Address.City,
                command.Address.Country);

            var addressResponse = await addressService.AddAsync(addressCommand, cancellationToken);
            addressId = addressResponse.Id;
        }

        var supplier = new SupplierModel
        {
            Id = command.Id,
            Person = person,
            PersonId = person.Id,
            Cnpj = command.Cnpj,
            CompanyId = companyId
        };

        await supplierRepository.InsertAsync(supplier, cancellationToken);

        if (addressId.HasValue)
        {
            var personAddress = new PersonAddressModel
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                AddressId = addressId.Value,
                CompanyId = companyId
            };
            await personAddressService.InsertAsync(personAddress, companyId, cancellationToken);
        }

        return supplier.MapToAddSupplierResponse();
    }

    public async Task<UpdateSupplierResponse?> UpdateAsync(
        UpdateSupplierCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        var supplier = await supplierRepository.GetByIdWithDetailsAsync(command.Id, cancellationToken);

        if (supplier is null)
        {
            return null;
        }

        supplier.Cnpj = command.Cnpj;
        supplier.CompanyId = companyId;
        supplier.Person.Name = command.Name;
        supplier.Person.Email = command.Email;
        supplier.Person.Document = command.Document;
        supplier.Person.PhoneNumber = command.PhoneNumber ?? string.Empty;
        supplier.Person.CompanyId = companyId;

        if (command.Address != null)
        {
            var existingPersonAddress = supplier.Person.PersonAddresses.FirstOrDefault();

            if (existingPersonAddress?.Address != null)
            {
                existingPersonAddress.Address.Street = command.Address.Street;
                existingPersonAddress.Address.Number = command.Address.Number;
                existingPersonAddress.Address.Complement = command.Address.Complement;
                existingPersonAddress.Address.Neighborhood = command.Address.Neighborhood;
                existingPersonAddress.Address.ZipCode = command.Address.ZipCode;
                existingPersonAddress.Address.City = command.Address.City;
                existingPersonAddress.Address.Country = command.Address.Country;
            }
            else
            {
                var addressCommand = new AddressCommand(
                    command.Address.Street,
                    command.Address.Number,
                    command.Address.Complement,
                    command.Address.Neighborhood,
                    command.Address.ZipCode,
                    command.Address.StateId,
                    command.Address.City,
                    command.Address.Country);

                var addressResponse = await addressService.AddAsync(addressCommand, cancellationToken);

                var newPersonAddress = new PersonAddressModel
                {
                    Id = Guid.NewGuid(),
                    PersonId = supplier.PersonId,
                    AddressId = addressResponse.Id,
                    CompanyId = companyId
                };
                await personAddressService.InsertAsync(newPersonAddress, companyId, cancellationToken);
            }
        }

        await supplierRepository.UpdateAsync(supplier.Id, supplier, cancellationToken);

        return supplier.MapToUpdateSupplierResponse();
    }

    public async Task DeleteAsync(
        DeleteSupplierCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        await supplierRepository.DeleteAsync(command.Id, cancellationToken);
    }

    public async Task<SupplierPerformanceResponse> GetPerformanceAsync(
        GetSupplierPerformanceQuery query,
        CancellationToken cancellationToken = default)
    {
        var productStats = await GetProductStatsAsync(cancellationToken);

        var supplierIds = productStats.Select(ps => ps.SupplierId).ToList();
        var supplierNames = await supplierRepository.GetSupplierNamesAsync(supplierIds, cancellationToken);

        var productsPerSupplier = productStats.Where(ps => supplierNames.ContainsKey(ps.SupplierId))
            .Select(ps => ps with { SupplierName = supplierNames[ps.SupplierId] }).OrderByDescending(x => x.TotalStockValue).Take(query.TopLimit).ToList();

        var recentStockMovements = await GetRecentStockMovementsAsync(query.Days, query.TopLimit, cancellationToken);

        var productsWithMultipleSuppliers = await GetCostComparisonAsync(query.TopLimit, cancellationToken);

        var summary = new SupplierSummaryResponse
        {
            TotalSuppliers = productsPerSupplier.Count,
            TotalProducts = productsPerSupplier.Sum(s => s.ProductCount),
            TotalStockValue = productsPerSupplier.Sum(s => s.TotalStockValue),
            AverageProductsPerSupplier = productsPerSupplier.Any()
                ? (decimal)productsPerSupplier.Sum(s => s.ProductCount) / productsPerSupplier.Count
                : 0
        };

        return new SupplierPerformanceResponse
        {
            ProductsPerSupplier = productsPerSupplier,
            CostComparison = productsWithMultipleSuppliers,
            RecentStockMovements = recentStockMovements,
            Summary = summary
        };
    }

    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return supplierRepository.CountAsync(cancellationToken);
    }

    public async Task<List<SupplierProductCountResponse>> GetProductStatsAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await productService.GetAllForStatsAsync(cancellationToken);
        var productList = products.ToList();

        return
        [
            .. productList
                .GroupBy(p => p.SupplierId!.Value)
                .Select(g => new SupplierProductCountResponse(
                    g.Key,
                    string.Empty,
                    g.Count(),
                    g.Sum(p => (p.CostPrice ?? 0m) * (decimal)p.Quantity),
                    g.Sum(p => p.SalesPrice * (decimal)p.Quantity)))
        ];
    }

    public async Task<List<SupplierStockMovementResponse>> GetRecentStockMovementsAsync(
        int days,
        int topLimit,
        CancellationToken cancellationToken = default)
    {
        var movements = await stockMovementService.GetRecentWithProductAsync(days, topLimit, cancellationToken);
        var movementList = movements.ToList();

        return
        [
            .. movementList.Select(m => new SupplierStockMovementResponse(
                m.Id,
                m.ProductId,
                m.Product.Name,
                m.Quantity,
                m.Price ?? 0,
                m.Date!.Value,
                m.Type.ToString()))
        ];
    }

    public async Task<List<SupplierCostComparisonResponse>> GetCostComparisonAsync(
        int topLimit,
        CancellationToken cancellationToken = default)
    {
        var products = await productService.GetAllWithSupplierAsync(cancellationToken);
        var productList = products.Where(p => p.SupplierId.HasValue).ToList();

        return
        [
            .. productList
                .GroupBy(p => p.Name)
                .Where(g => g.Count() > 1)
                .Select(g => new SupplierCostComparisonResponse(
                    g.Key,
                    [
                        .. g.Select(p => new ProductSupplierPriceResponse(
                            p.SupplierId!.Value,
                            p.Supplier!.Person.Name,
                            p.CostPrice ?? 0,
                            p.SalesPrice,
                            p.SalesPrice > 0 ? (p.SalesPrice - (p.CostPrice ?? 0)) / p.SalesPrice * 100 : 0))
                    ]))
                .Take(topLimit)
        ];
    }

    public async Task<List<SupplierBreakdownResponse>> GetSupplierBreakdownAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await productService.GetAllWithSupplierAsync(cancellationToken);
        var productList = products.Where(p => p.SupplierId.HasValue).ToList();

        return
        [
            .. productList
                .GroupBy(p => new { SupplierId = p.SupplierId!.Value, SupplierName = p.Supplier!.Person.Name })
                .Select(g => new SupplierBreakdownResponse(
                    g.Key.SupplierId,
                    g.Key.SupplierName,
                    g.Sum(p => (p.CostPrice ?? 0m) * (decimal)p.Quantity),
                    g.Sum(p => p.SalesPrice * (decimal)p.Quantity),
                    g.Sum(p => p.Quantity)))
                .OrderByDescending(s => s.TotalSalesValue)
        ];
    }

    public async Task<List<GetSupplierByIdResponse>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        var suppliers = await supplierRepository.Query()
            .Where(s => idList.Contains(s.Id))
            .Include(s => s.Person)
            .ThenInclude(p => p.PersonAddresses)
            .ThenInclude(pa => pa.Address)
            .ToListAsync(cancellationToken);

        return
        [
            .. suppliers.Select(s =>
            {
                var personAddress = s.Person.PersonAddresses.FirstOrDefault();
                var address = personAddress?.Address;

                AddressResponse? addressResponse = null;
                if (address != null)
                {
                    addressResponse = new AddressResponse(
                        address.Id,
                        address.Street,
                        address.Number,
                        address.Complement,
                        address.Neighborhood,
                        address.ZipCode!,
                        address.StateId,
                        address.State.Name,
                        address.City,
                        address.Country);
                }

                return new GetSupplierByIdResponse(
                    s.Id,
                    s.PersonId,
                    s.Person.Name,
                    s.Person.Email,
                    s.Person.PhoneNumber,
                    s.Person.Document,
                    addressResponse);
            })
        ];
    }
}