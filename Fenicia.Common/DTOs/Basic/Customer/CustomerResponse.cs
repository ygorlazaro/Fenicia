using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.Address;

namespace Fenicia.Common.DTOs.Basic.Customer;

/// <summary>
/// Response DTO containing customer details.
/// </summary>
public class CustomerResponse() : ICrudItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerResponse"/> class with the specified customer details.
    /// </summary>
    /// <param name="id">The unique identifier of the customer.</param>
    /// <param name="personId">The unique identifier of the associated person.</param>
    /// <param name="name">The customer name.</param>
    /// <param name="email">The customer email (optional).</param>
    /// <param name="phoneNumber">The customer phone number (optional).</param>
    /// <param name="document">The customer document (optional).</param>
    /// <param name="address">The customer address (optional).</param>
    public CustomerResponse(
        Guid id,
        Guid personId,
        string name,
        string? email,
        string? phoneNumber,
        string? document,
        AddressResponse? address)
        : this()
    {
        Id = id;
        PersonId = personId;
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        Document = document;
        Address = address;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the customer.
    /// </summary>
    [Required]
    public Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the unique identifier of the associated person.
    /// </summary>
    [Required]
    public Guid PersonId { get; set; }

    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer email.
    /// </summary>
    [MaxLength(50)]
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the customer phone number.
    /// </summary>
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the customer document.
    /// </summary>
    [MaxLength(14)]
    public string? Document { get; set; }

    /// <summary>
    /// Gets or sets the customer address.
    /// </summary>
    public AddressResponse? Address { get; set; }
}