using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.DTOs.Basic.Supplier;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Basic.Domains.Supplier;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class SupplierMapper
{
    [MapProperty("Person.Name", nameof(GetAllSupplierResponse.Name))]
    [MapProperty("Person.Email", nameof(GetAllSupplierResponse.Email))]
    [MapProperty("Person.PhoneNumber", nameof(GetAllSupplierResponse.PhoneNumber))]
    [MapProperty("Person.Document", nameof(GetAllSupplierResponse.Document))]
    public partial GetAllSupplierResponse MapToGetAllSupplierResponse(SupplierModel supplier);

    [MapProperty("Person.Name", nameof(GetSupplierByIdResponse.Name))]
    [MapProperty("Person.Email", nameof(GetSupplierByIdResponse.Email))]
    [MapProperty("Person.PhoneNumber", nameof(GetSupplierByIdResponse.PhoneNumber))]
    [MapProperty("Person.Document", nameof(GetSupplierByIdResponse.Document))]
    public partial GetSupplierByIdResponse MapToGetSupplierByIdResponse(SupplierModel supplier);

    public partial AddSupplierResponse MapToAddSupplierResponse(SupplierModel supplier);

    public partial UpdateSupplierResponse MapToUpdateSupplierResponse(SupplierModel supplier);
}
