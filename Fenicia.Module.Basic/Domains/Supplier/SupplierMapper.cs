using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Supplier;
using Fenicia.Common.DTOs.Auth.Address;

namespace Fenicia.Module.Basic.Domains.Supplier;

/// <summary>
/// Mapper class for mapping between SupplierModel and Supplier DTOs.
/// </summary>
public static class SupplierMapper
{
    /// <summary>
    /// Maps a SupplierModel to an AddSupplierResponse.
    /// </summary>
    /// <param name="supplier">The supplier model.</param>
    /// <returns>The add supplier response.</returns>
    public static AddSupplierResponse MapToAddSupplierResponse(SupplierModel supplier)
    {
        return new AddSupplierResponse(supplier.Id, supplier.Cnpj);
    }

    /// <summary>
    /// Maps a SupplierModel to a GetAllSupplierResponse.
    /// </summary>
    /// <param name="supplier">The supplier model.</param>
    /// <param name="address">The supplier's address.</param>
    /// <returns>The get all supplier response.</returns>
    public static GetAllSupplierResponse MapToGetAllSupplierResponse(SupplierModel supplier, AddressResponse? address = null)
    {
        return new GetAllSupplierResponse(
            supplier.Id,
            supplier.PersonId,
            supplier.Person.Name,
            supplier.Person.Email,
            supplier.Person.PhoneNumber,
            supplier.Person.Document,
            address);
    }

    /// <summary>
    /// Maps a SupplierModel to a SupplierSummaryResponse.
    /// </summary>
    /// <param name="supplier">The supplier model.</param>
    /// <returns>The supplier summary response.</returns>
    public static SupplierSummaryResponse MapToSupplierSummaryResponse(SupplierModel supplier)
    {
        return new SupplierSummaryResponse
        {
            TotalSuppliers = supplier.Person != null ? 1 : 0,
            TotalProducts = 0,
            TotalStockValue = 0,
            AverageProductsPerSupplier = 0
        };
    }
}
