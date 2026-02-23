namespace Fenicia.Module.Basic.Domains.Supplier;

public record GetAllSupplierQuery(int Page = 1, int PerPage = 10);
public record GetSupplierByIdQuery(Guid Id);
public record AddSupplierCommand(
    Guid Id,
    string Name,
    string? Email,
    string? Cpf,
    string? Phone,
    string? City,
    string? Complement,
    string? Neighborhood,
    string? Number,
    Guid StateId,
    string? Street,
    string? ZipCode,
    string? PhoneNumber,
    string? Cnpj);
public record UpdateSupplierCommand(
    Guid Id,
    string Name,
    string? Email,
    string? Cpf,
    string? Phone,
    string? City,
    string? Complement,
    string? Neighborhood,
    string? Number,
    Guid StateId,
    string? Street,
    string? ZipCode,
    string? PhoneNumber,
    string? Cnpj);
public record DeleteSupplierCommand(Guid Id);

public record SupplierResponse(
    Guid Id,
    string? Cnpj,
    PersonResponse Person);

public record PersonResponse(
    string Name,
    string? Email,
    string? Cpf,
    string? Phone,
    AddressResponse? Address);

public record AddressResponse(
    string? City,
    string? Complement,
    string? Neighborhood,
    string? Number,
    Guid StateId,
    string? Street,
    string? ZipCode);
