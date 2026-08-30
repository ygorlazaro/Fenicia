using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier;

public class SupplierService(
    SupplierRepository supplierRepository,
    ProductService productService,
    StockMovementService stockMovementService,
    AddressService addressService,
    PersonAddressService personAddressService)
{
    public async Task<Pagination<List<GetAllSupplierResponse>>> GetAllAsync(GetAllSupplierQuery query, CancellationToken ct)
    {
        var total = await supplierRepository.CountAsync(ct);

        var suppliers = await supplierRepository.GetAllWithDetailsAsync(query.Page, query.PerPage, ct);

        var response = suppliers.Select(s =>
    {
        var personAddress = s.Person.PersonAddresses.FirstOrDefault();
        var address = personAddress?.Address;

        return new GetAllSupplierResponse(
            s.Id,
            s.PersonId,
            s.Person.Name,
            s.Person.Email,
            s.Person.PhoneNumber,
            s.Person.Document,
            address != null ? new AddressResponse(address.Id, address.Street, address.Number, address.Complement, address.Neighborhood, address.ZipCode!, address.StateId, address.State?.Name, address.City, address.Country) : null);
    }).ToList();

        return new Pagination<List<GetAllSupplierResponse>>(response, total, query.Page, query.PerPage);
    }

    public async Task<List<GetAllSupplierForDataSourceResponse>> GetAllForDataSourceAsync(CancellationToken ct)
    {
        var suppliers = await supplierRepository.GetAllWithDetailsAsync(ct: ct);

        return suppliers.Select(s => new GetAllSupplierForDataSourceResponse(s.Id, s.Person.Name)).ToList();
    }

    public async Task<GetSupplierByIdResponse?> GetByIdAsync(GetSupplierByIdQuery query, CancellationToken ct)
    {
        var supplier = await supplierRepository.GetByIdWithDetailsAsync(query.Id, ct);

        if (supplier is null)
        {
            return null;
        }

        var personAddress = supplier.Person.PersonAddresses.FirstOrDefault();
        var address = personAddress?.Address;

        return new GetSupplierByIdResponse(
            supplier.Id,
            supplier.PersonId,
            supplier.Person.Name,
            supplier.Person.Email,
            supplier.Person.PhoneNumber,
            supplier.Person.Document,
            address != null ? new AddressResponse(address.Id, address.Street, address.Number, address.Complement, address.Neighborhood, address.ZipCode!, address.StateId, address.State?.Name, address.City, address.Country) : null);
    }

    public async Task<AddSupplierResponse> AddAsync(AddSupplierCommand command, Guid companyId, CancellationToken ct)
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

            var addressResponse = await addressService.AddAsync(addressCommand, ct);
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

        if (addressId.HasValue)
        {
            var personAddress = new PersonAddressModel
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                AddressId = addressId.Value,
                CompanyId = companyId
            };
            await personAddressService.InsertAsync(personAddress, companyId, ct);
        }

        await supplierRepository.InsertAsync(supplier, ct);

        return new AddSupplierResponse(supplier.Id, supplier.Cnpj);
    }

    public async Task<UpdateSupplierResponse?> UpdateAsync(UpdateSupplierCommand command, Guid companyId, CancellationToken ct)
    {
        var supplier = await supplierRepository.GetByIdWithDetailsAsync(command.Id, ct);

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
                existingPersonAddress.Address.StateId = command.Address.StateId;
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

                var addressResponse = await addressService.AddAsync(addressCommand, ct);

                var newPersonAddress = new PersonAddressModel
                {
                    Id = Guid.NewGuid(),
                    PersonId = supplier.PersonId,
                    AddressId = addressResponse.Id,
                    CompanyId = companyId
                };
                await personAddressService.InsertAsync(newPersonAddress, companyId, ct);
            }
        }

        await supplierRepository.UpdateAsync(supplier.Id, supplier, ct);

        return new UpdateSupplierResponse(supplier.Id, supplier.Cnpj);
    }

    public async Task DeleteAsync(DeleteSupplierCommand command, Guid companyId, CancellationToken ct)
    {
        var supplier = await supplierRepository.GetByIdAsync(command.Id, ct);

        if (supplier is null)
        {
            return;
        }

        supplier.Deleted = DateTime.Now;
        supplier.CompanyId = companyId;

        await supplierRepository.UpdateAsync(supplier.Id, supplier, ct);
    }

    public async Task<SupplierPerformanceResponse> GetPerformanceAsync(GetSupplierPerformanceQuery query, CancellationToken ct)
    {
        var productStats = await GetProductStatsAsync(ct);

        var supplierIds = productStats.Select(ps => ps.SupplierId).ToList();
        var supplierNames = await supplierRepository.GetSupplierNamesAsync(supplierIds, ct);

        var productsPerSupplier = productStats.Where(ps => supplierNames.ContainsKey(ps.SupplierId)).Select(ps => new SupplierProductCountResponse(ps.SupplierId, supplierNames[ps.SupplierId], ps.ProductCount, ps.TotalStockValue, ps.TotalRevenue)).OrderByDescending(x => x.TotalStockValue).Take(query.TopLimit).ToList();

        var recentStockMovements = await GetRecentStockMovementsAsync(query.Days, query.TopLimit, ct);

        var productsWithMultipleSuppliers = await GetCostComparisonAsync(query.TopLimit, ct);

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

    public async Task<int> GetCountAsync(CancellationToken ct)
    {
        return await supplierRepository.CountAsync(ct);
    }

    public async Task<List<SupplierProductCountResponse>> GetProductStatsAsync(CancellationToken ct)
    {
        var products = await productService.GetAllForStatsAsync(ct);
        var productList = products.ToList();

        return productList
            .GroupBy(p => p.SupplierId!.Value)
            .Select(g => new SupplierProductCountResponse(
                g.Key,
                string.Empty,
                g.Count(),
                g.Sum(p => (p.CostPrice ?? 0m) * (decimal)p.Quantity),
                g.Sum(p => p.SalesPrice * (decimal)p.Quantity)))
            .ToList();
    }

    public async Task<List<SupplierStockMovementResponse>> GetRecentStockMovementsAsync(int days, int topLimit, CancellationToken ct)
    {
        var movements = await stockMovementService.GetRecentWithProductAsync(days, topLimit, ct);
        var movementList = movements.ToList();

        return movementList.Select(m => new SupplierStockMovementResponse(
            m.Id,
            m.ProductId,
            m.Product.Name,
            m.Quantity,
            m.Price ?? 0,
            m.Date!.Value,
            m.Type.ToString())).ToList();
    }

    public async Task<List<SupplierCostComparisonResponse>> GetCostComparisonAsync(int topLimit, CancellationToken ct)
    {
        var products = await productService.GetAllWithSupplierAsync(ct);
        var productList = products.Where(p => p.SupplierId.HasValue).ToList();

        return productList
            .GroupBy(p => p.Name)
            .Where(g => g.Count() > 1)
            .Select(g => new SupplierCostComparisonResponse(
                g.Key,
                g.Select(p => new ProductSupplierPriceResponse(
                    p.SupplierId!.Value,
                    p.Supplier!.Person.Name,
                    p.CostPrice ?? 0,
                    p.SalesPrice,
                    p.SalesPrice > 0 ? (p.SalesPrice - (p.CostPrice ?? 0)) / p.SalesPrice * 100 : 0)).ToList()))
            .Take(topLimit)
            .ToList();
    }

    public async Task<List<SupplierBreakdownResponse>> GetSupplierBreakdownAsync(CancellationToken ct)
    {
        var products = await productService.GetAllWithSupplierAsync(ct);
        var productList = products.Where(p => p.SupplierId.HasValue).ToList();

        return productList
            .GroupBy(p => new { SupplierId = p.SupplierId!.Value, SupplierName = p.Supplier!.Person.Name })
            .Select(g => new SupplierBreakdownResponse(
                g.Key.SupplierId,
                g.Key.SupplierName,
                g.Sum(p => (p.CostPrice ?? 0m) * (decimal)p.Quantity),
                g.Sum(p => p.SalesPrice * (decimal)p.Quantity),
                g.Sum(p => p.Quantity)))
            .OrderByDescending(s => s.TotalSalesValue)
            .ToList();
    }

    public async Task<List<GetSupplierByIdResponse>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.ToList();
        var suppliers = await supplierRepository.Query()
            .Where(s => idList.Contains(s.Id) && s.Deleted == null)
            .Include(s => s.Person)
                .ThenInclude(p => p.PersonAddresses)
                    .ThenInclude(pa => pa.Address)
            .ToListAsync(ct);

        return suppliers.Select(s =>
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
                    address.State?.Name,
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
        }).ToList();
    }
}
