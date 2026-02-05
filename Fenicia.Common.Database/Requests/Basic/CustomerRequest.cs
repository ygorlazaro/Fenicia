using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Database.Requests.Basic;

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

    public string City
    {
        get;
        set;
    }

    public string Complement
    {
        get;
        set;
    }

    public string Neighborhood
    {
        get;
        set;
    }

    public string Number
    {
        get;
        set;
    }

    public Guid StateId
    {
        get;
        set;
    }

    public string Street
    {
        get;
        set;
    }

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
