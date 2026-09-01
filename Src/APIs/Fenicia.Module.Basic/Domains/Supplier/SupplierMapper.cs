using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Supplier;

[Mapper]
public static partial class SupplierMapper
{
    public static GetAllSupplierResponse MapToGetAllSupplierResponse(this SupplierModel supplier)
    {
        var personAddress = supplier.Person.PersonAddresses.FirstOrDefault();
        var address = personAddress?.Address;

        return new GetAllSupplierResponse(
            supplier.Id,
            supplier.PersonId,
            supplier.Person.Name,
            supplier.Person.Email,
            supplier.Person.PhoneNumber,
            supplier.Person.Document,
            address != null ? new AddressResponse(address.Id, address.Street, address.Number, address.Complement, address.Neighborhood, address.ZipCode!, address.StateId, address.State.Name, address.City, address.Country) : null);
    }

    public static GetSupplierByIdResponse MapToGetSupplierByIdResponse(this SupplierModel supplier)
    {
        var personAddress = supplier.Person.PersonAddresses.FirstOrDefault();
        var address = personAddress?.Address;

        return new GetSupplierByIdResponse(
            supplier.Id,
            supplier.PersonId,
            supplier.Person.Name,
            supplier.Person.Email,
            supplier.Person.PhoneNumber,
            supplier.Person.Document,
            address != null ? new AddressResponse(address.Id, address.Street, address.Number, address.Complement, address.Neighborhood, address.ZipCode!, address.StateId, address.State.Name, address.City, address.Country) : null);
    }

    public static AddSupplierResponse MapToAddSupplierResponse(this SupplierModel supplier)
    {
        return new AddSupplierResponse(supplier.Id, supplier.Cnpj);
    }

    public static UpdateSupplierResponse MapToUpdateSupplierResponse(this SupplierModel supplier)
    {
        return new UpdateSupplierResponse(supplier.Id, supplier.Cnpj);
    }
}
