using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.Services;

public class SupplierService(DefaultContext db)
{
    public async Task<Pagination<List<GetAllSupplierResponse>>> GetAllAsync(GetAllSupplierQuery query, CancellationToken ct)
    {
        var total = await db.BasicSuppliers.CountAsync(ct);

        var suppliers = await db.BasicSuppliers
            .Include(s => s.Person)
            .Include(s => s.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

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
                address != null ? new AddressResponse(
                    address.Id,
                    address.Street,
                    address.Number,
                    address.Complement,
                    address.Neighborhood,
                    address.ZipCode,
                    address.StateId,
                    address.State?.Name,
                    address.City,
                    address.Country
                ) : null
            );
        }).ToList();

        return new Pagination<List<GetAllSupplierResponse>>(response, total, query.Page, query.PerPage);
    }

    public async Task<GetSupplierByIdResponse?> GetByIdAsync(GetSupplierByIdQuery query, CancellationToken ct)
    {
        var supplier = await db.BasicSuppliers
            .Include(s => s.Person)
            .Include(s => s.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .FirstOrDefaultAsync(s => s.Id == query.Id, ct);

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
            address != null ? new AddressResponse(
                address.Id,
                address.Street,
                address.Number,
                address.Complement,
                address.Neighborhood,
                address.ZipCode,
                address.StateId,
                address.State?.Name,
                address.City,
                address.Country
            ) : null
        );
    }

    public async Task<AddSupplierResponse> AddAsync(AddSupplierCommand command, CancellationToken ct)
    {
        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            Document = command.Document,
            PhoneNumber = command.PhoneNumber
        };

        AddressModel? address = null;

        if (command.Address != null)
        {
            address = new AddressModel
            {
                Id = Guid.NewGuid(),
                Street = command.Address.Street,
                Number = command.Address.Number,
                Complement = command.Address.Complement,
                Neighborhood = command.Address.Neighborhood,
                ZipCode = command.Address.ZipCode,
                StateId = command.Address.StateId,
                City = command.Address.City,
                Country = command.Address.Country
            };
            db.AuthAddresses.Add(address);
        }

        var supplier = new SupplierModel
        {
            Id = command.Id,
            Person = person,
            PersonId = person.Id,
            Cnpj = command.Cnpj
        };

        if (address != null)
        {
            var personAddress = new PersonAddressModel
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                AddressId = address.Id
            };
            db.BasicPersonAddresses.Add(personAddress);
        }

        db.BasicSuppliers.Add(supplier);

        await db.SaveChangesAsync(ct);

        return new AddSupplierResponse(supplier.Id, supplier.Cnpj);
    }

    public async Task<UpdateSupplierResponse?> UpdateAsync(UpdateSupplierCommand command, CancellationToken ct)
    {
        var supplier = await db.BasicSuppliers
            .Include(s => s.Person)
            .Include(s => s.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
            .FirstOrDefaultAsync(s => s.Id == command.Id, ct);

        if (supplier is null)
        {
            return null;
        }

        supplier.Cnpj = command.Cnpj;
        supplier.Person.Name = command.Name;
        supplier.Person.Email = command.Email;
        supplier.Person.Document = command.Document;
        supplier.Person.PhoneNumber = command.PhoneNumber ?? string.Empty;

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
                var newAddress = new AddressModel
                {
                    Id = Guid.NewGuid(),
                    Street = command.Address.Street,
                    Number = command.Address.Number,
                    Complement = command.Address.Complement,
                    Neighborhood = command.Address.Neighborhood,
                    ZipCode = command.Address.ZipCode,
                    StateId = command.Address.StateId,
                    City = command.Address.City,
                    Country = command.Address.Country
                };
                db.AuthAddresses.Add(newAddress);

                var newPersonAddress = new PersonAddressModel
                {
                    Id = Guid.NewGuid(),
                    PersonId = supplier.PersonId,
                    AddressId = newAddress.Id
                };
                db.BasicPersonAddresses.Add(newPersonAddress);
            }
        }

        db.BasicSuppliers.Update(supplier);

        await db.SaveChangesAsync(ct);

        return new UpdateSupplierResponse(supplier.Id, supplier.Cnpj);
    }

    public async Task DeleteAsync(DeleteSupplierCommand command, CancellationToken ct)
    {
        var supplier = await db.BasicSuppliers.FirstOrDefaultAsync(s => s.Id == command.Id, ct);

        if (supplier is null)
        {
            return;
        }

        supplier.Deleted = DateTime.Now;

        db.BasicSuppliers.Update(supplier);

        await db.SaveChangesAsync(ct);
    }

    public async Task<SupplierPerformanceResponse> GetPerformanceAsync(GetSupplierPerformanceQuery query, CancellationToken ct)
    {

        var productStats = await db.BasicProducts.Where(p => p.SupplierId.HasValue).GroupBy(p => p.SupplierId!.Value).Select(g => new { SupplierId = g.Key, ProductCount = g.Count(), TotalCostValue = g.Sum(p => (p.CostPrice ?? 0m) * (decimal)p.Quantity), TotalSalesValue = g.Sum(p => p.SalesPrice * (decimal)p.Quantity) }).ToListAsync(ct);

        var supplierNames = await db.BasicSuppliers.Include(s => s.Person).Where(s => productStats.Select(ps => ps.SupplierId).Contains(s.Id)).Select(s => new { s.Id, s.Person.Name }).ToDictionaryAsync(s => s.Id, s => s.Name, ct);

        var productsPerSupplier = productStats.Where(ps => supplierNames.ContainsKey(ps.SupplierId)).Select(ps => new SupplierProductCountResponse(ps.SupplierId, supplierNames[ps.SupplierId], ps.ProductCount, ps.TotalCostValue, ps.TotalSalesValue)).OrderByDescending(x => x.TotalStockValue).Take(query.TopLimit).ToList();

        var recentStockMovementsQuery = db.BasicStockMovements.Include(m => m.Product).Where(m => m.SupplierId.HasValue && m.Date >= DateTime.UtcNow.AddDays(-query.Days)).OrderByDescending(m => m.Date).Take(query.TopLimit).Select(m => new SupplierStockMovementResponse(m.Id, m.ProductId, m.Product.Name, m.Quantity, m.Price ?? 0, m.Date!.Value, m.Type.ToString()));

        var recentStockMovements = await recentStockMovementsQuery.ToListAsync(ct);

        var productsWithMultipleSuppliers = await GetSupplierCostComparisonAsync(query, ct);

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

    private async Task<List<SupplierCostComparisonResponse>> GetSupplierCostComparisonAsync(GetSupplierPerformanceQuery query, CancellationToken ct)
    {
        var productsWithMultipleSuppliers = await db.BasicProducts.Include(p => p.Supplier).ThenInclude(s => s.Person).Where(p => p.SupplierId.HasValue).GroupBy(p => p.Name).Where(g => g.Count() > 1).Select(g => new SupplierCostComparisonResponse(g.Key, g.Select(p => new ProductSupplierPriceResponse(p.SupplierId!.Value, p.Supplier!.Person.Name, p.CostPrice ?? 0, p.SalesPrice, p.SalesPrice > 0 ? (p.SalesPrice - (p.CostPrice ?? 0)) / p.SalesPrice * 100 : 0)).ToList())).Take(query.TopLimit)
            .ToListAsync(ct);

        return productsWithMultipleSuppliers;
    }
}
