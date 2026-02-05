using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Database.Requests.Basic;

public class AddressRequest
{
    [Required]
    [MaxLength(100, ErrorMessage = "Street cannot exceed 100 characters")]
    public string Street
    {
        get;
        set;
    }

    [Required]
    [MaxLength(10, ErrorMessage = "Number cannot exceed 10 characters")]
    public string Number
    {
        get;
        set;
    }

    [MaxLength(10, ErrorMessage = "Complement cannot exceed 10 characters")]
    public string Complement
    {
        get;
        set;
    }

    [Required]
    [MaxLength(50, ErrorMessage = "Neighborhood cannot exceed 50 characters")]
    public string Neighborhood
    {
        get;
        set;
    }

    [Required]
    [MaxLength(50, ErrorMessage = "City cannot exceed 50 characters")]
    public string City
    {
        get;
        set;
    }

    [Required]
    public Guid State
    {
        get;
        set;
    }

    [Required]
    [MaxLength(9, ErrorMessage = "ZipCode cannot exceed 9 characters")]
    public string ZipCode
    {
        get;
        set;
    }
}
