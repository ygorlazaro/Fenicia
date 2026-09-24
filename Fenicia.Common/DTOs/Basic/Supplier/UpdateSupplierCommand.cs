using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Supplier;

public class UpdateSupplierRequest()
{
    public UpdateSupplierRequest(
        Guid id,
        string name,
        string? email,
        string? document,
        string? phoneNumber,
        string? cnpj,
        AddressDTO? address)
        : this()
    {
        Id = id;
        Name = name;
        Email = email;
        Document = document;
        PhoneNumber = phoneNumber;
        Cnpj = cnpj;
        Address = address;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(14)]
    public string? Document { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(14)]
    public string? Cnpj { get; set; }

    public AddressDTO? Address { get; set; }
}
