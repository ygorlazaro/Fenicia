using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Fenicia.Common.Data.Requests.Basic;

public class SupplierRequest
{
    public string? Cpf
    {
        get;
        set;
    }

    [MaxLength(50)]
    [Required]
    public string Name
    {
        get;
        set;
    }

    public Guid Id
    {
        get;
        set;
    }

    [MaxLength(50)]
    [Required]
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

    [Required]
    [MaxLength(8)]
    public string ZipCode
    {
        get;
        set;
    }
}
