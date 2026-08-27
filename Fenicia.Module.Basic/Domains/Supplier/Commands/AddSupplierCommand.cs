using Fenicia.Module.Basic.Domains.Supplier.Common;
using MediatR;
using Fenicia.Module.Basic.Domains.Supplier.Responses;

namespace Fenicia.Module.Basic.Domains.Supplier.Commands;

public record AddSupplierCommand(

Guid Id,

string Name,

string? Email,

string? Document,

string? PhoneNumber,

string? Cnpj,

AddressDTO? Address) : IRequest<AddSupplierResponse>;