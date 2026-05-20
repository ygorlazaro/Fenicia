using Fenicia.Module.Basic.Domains.Supplier.Common;
using MediatR;
using Fenicia.Module.Basic.Domains.Supplier.Responses;

namespace Fenicia.Module.Basic.Domains.Supplier.Commands;

/// <summary>
///     Command record for updating an existing supplier.
/// </summary>
public record UpdateSupplierCommand(
    /// <summary>
/// Unique identifier of the supplier to update.
/// </summary>
Guid Id,
    /// <summary>
/// Updated name of the supplier.
/// </summary>
string Name,
    /// <summary>
/// Updated email address.
/// </summary>
string? Email,
    /// <summary>
/// Updated document number.
/// </summary>
string? Document,
    /// <summary>
/// Updated phone number.
/// </summary>
string? PhoneNumber,
    /// <summary>
/// Updated CNPJ number.
/// </summary>
string? Cnpj,
    /// <summary>
/// Updated address.
/// </summary>
AddressDTO? Address) : IRequest<UpdateSupplierResponse?>;