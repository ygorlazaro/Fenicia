using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Person;

/// <summary>
/// Response DTO containing person details.
/// </summary>
public class PersonResponse()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PersonResponse"/> class with the specified person details.
    /// </summary>
    /// <param name="id">The unique identifier of the person.</param>
    /// <param name="name">The person's name.</param>
    /// <param name="document">The person's document number (optional).</param>
    /// <param name="email">The person's email address (optional).</param>
    /// <param name="phoneNumber">The person's phone number (optional).</param>
    /// <param name="dateOfBirth">The person's date of birth (optional).</param>
    /// <param name="photoUrl">The person's photo URL (optional).</param>
    /// <param name="notes">Additional notes about the person (optional).</param>
    public PersonResponse(
        Guid id,
        string name,
        string? document,
        string? email,
        string? phoneNumber,
        DateTime? dateOfBirth,
        string? photoUrl,
        string? notes)
        : this()
    {
        Id = id;
        Name = name;
        Document = document;
        Email = email;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        PhotoUrl = photoUrl;
        Notes = notes;
    }

    /// <summary>
    /// Gets the unique identifier of the person.
    /// </summary>
    [Required]
    public Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the person's name.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the person's document number.
    /// </summary>
    [MaxLength(14)]
    public string? Document { get; set; }

    /// <summary>
    /// Gets or sets the person's email address.
    /// </summary>
    [MaxLength(50)]
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the person's phone number.
    /// </summary>
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the person's date of birth.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; init; }

    /// <summary>
    /// Gets or sets the person's photo URL.
    /// </summary>
    [MaxLength(500)]
    public string? PhotoUrl { get; init; }

    /// <summary>
    /// Gets or sets additional notes about the person.
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; init; }
}
