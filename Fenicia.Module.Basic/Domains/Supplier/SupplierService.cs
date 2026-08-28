using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier;

public class SupplierService(SupplierRepository supplierRepository)
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
            address != null ? new AddressResponse(address.Id, address.Street, address.Number, address.Complement, address.Neighborhood, address.ZipCode, address.StateId, address.State?.Name, address.City, address.Country) : null);
    }).ToList();

        return new Pagination<List<GetAllSupplierResponse>>(response, total, query.Page, query.PerPage);
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
            address != null ? new AddressResponse(address.Id, address.Street, address.Number, address.Complement, address.Neighborhood, address.ZipCode, address.StateId, address.State?.Name, address.City, address.Country) : null);
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
            supplierRepository.Context.AuthAddresses.Add(address);
        }

        var supplier = new SupplierModel
        {
            Id = command.Id,
            Person = person,
            PersonId = person.Id,
            Cnpj = command.Cnpj,
            CompanyId = companyId
        };

        if (address != null)
        {
            var personAddress = new PersonAddressModel
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                AddressId = address.Id,
                CompanyId = companyId
            };
            supplierRepository.Context.BasicPersonAddresses.Add(personAddress);
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
                supplierRepository.Context.AuthAddresses.Add(newAddress);

                var newPersonAddress = new PersonAddressModel
                {
                    Id = Guid.NewGuid(),
                    PersonId = supplier.PersonId,
                    AddressId = newAddress.Id,
                    CompanyId = companyId
                };
                supplierRepository.Context.BasicPersonAddresses.Add(newPersonAddress);
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
        var productStats = await supplierRepository.GetProductStatsAsync(ct);

        var supplierIds = productStats.Select(ps => ps.SupplierId).ToList();
        var supplierNames = await supplierRepository.GetSupplierNamesAsync(supplierIds, ct);

        var productsPerSupplier = productStats.Where(ps => supplierNames.ContainsKey(ps.SupplierId)).Select(ps => new SupplierProductCountResponse(ps.SupplierId, supplierNames[ps.SupplierId], ps.ProductCount, ps.TotalStockValue, ps.TotalRevenue)).OrderByDescending(x => x.TotalStockValue).Take(query.TopLimit).ToList();

        var recentStockMovements = await supplierRepository.GetRecentStockMovementsAsync(query.Days, query.TopLimit, ct);

        var productsWithMultipleSuppliers = await supplierRepository.GetCostComparisonAsync(query.TopLimit, ct);

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
}
