namespace Fenicia.Module.Basic.Domains.Customer.Add;

public record AddCustomerCommand(
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
    string? PhoneNumber);
