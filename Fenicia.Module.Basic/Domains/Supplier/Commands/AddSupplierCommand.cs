using Fenicia.Module.Basic.Domains.Supplier.Common;
using MediatR;
using Fenicia.Module.Basic.Domains.Supplier.Responses;

namespace Fenicia.Module.Basic.Domains.Supplier.Commands;

/// <summary>
///     Command record for creating a new supplier.
/// </summary>
public record AddSupplierCommand(
    /// <summary>
/// Unique identifier for the new supplier.
/// </summary>
Guid Id,
    /// <summary>
/// Name of the supplier.
/// </summary>
string Name,
    /// <summary>
/// Email address (optional).
/// </summary>
string? Email,
    /// <summary>
/// Document number (CPF/CNPJ).
/// </summary>
string? Document,
    /// <summary>
/// Phone number.
/// </summary>
string? PhoneNumber,
    /// <summary>
/// CNPJ number.
/// </summary>
string? Cnpj,
    /// <summary>
/// Address (optional).
/// </summary>
AddressDTO? Address) : IRequest<AddSupplierResponse>;