using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Person;

public class PersonRequest()
{
    public PersonRequest(
        string name,
        string? document,
        string? email,
        string? phoneNumber,
        DateTime? dateOfBirth,
        string? photoUrl,
        string? notes)
        : this()
    {
        Name = name;
        Document = document;
        Email = email;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        PhotoUrl = photoUrl;
        Notes = notes;
    }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(14)]
    public string? Document { get; set; }

    [MaxLength(50)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; init; }

    [MaxLength(500)]
    public string? PhotoUrl { get; init; }

    [MaxLength(1000)]
    public string? Notes { get; init; }
}
