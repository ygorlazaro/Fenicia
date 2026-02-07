using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data.Requests.Basic;

public class EmployeeRequest
{
    public Guid Id
    {
        get;
        set;
    }

    [Required]
    [MaxLength(50)]
    public string Name
    {
        get;
        set;
    }

    public string? Cpf
    {
        get;
        set;
    }

    [Required]
    public Guid PositionId
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

    [Required]
    [MaxLength(50)]
    public string Neighborhood
    {
        get;
        set;
    }

    [Required]
    [MaxLength(11)]
    public string Number
    {
        get;
        set;
    }

    [Required]
    [MaxLength(2)]
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
