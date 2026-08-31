using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Basic.Domains.Address.DTOs;

public record AddressCommand(
    [Required][MaxLength(200)] string Street,
    [Required][MaxLength(200)] string Number,
    string? Complement,
    string? Neighborhood,
    [Required][MaxLength(200)] string ZipCode,
    [Required] Guid StateId,
    [Required][MaxLength(200)] string City,
    string? Country);
