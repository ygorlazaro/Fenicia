using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data.Requests.Basic;

public class CustomerRequest
{
    [Required(ErrorMessage = "Customer name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Customer name must be between 2 and 50 characters")]
    public string Name
    {
        get;
        set;
    }

    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF must have exactly 11 characters")]
    public string Cpf
    {
        get;
        set;
    }

    [Required]
    [MaxLength(50)]
    public string City
    {
        get;
        set;
    }

    [MaxLength(50)]
    public string Complement
    {
        get;
        set;
    }

    [MaxLength(50)]
    [Required]
    public string Neighborhood
    {
        get;
        set;
    }

    [MaxLength(50)]
    [Required]
    public string Number
    {
        get;
        set;
    }

    [Required]
    public Guid StateId
    {
        get;
        set;
    }

    [Required]
    [MaxLength(50)]
    public string Street
    {
        get;
        set;
    }

    [MaxLength(58)]
    [Required]
    public string ZipCode
    {
        get;
        set;
    }

    public Guid Id
    {
        get;
        set;
    }
}
