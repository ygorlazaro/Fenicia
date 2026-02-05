using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("suppliers")]
public class SupplierModel : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [MaxLength(14)]
    public string? Cpf { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Street
    {
        get;
        set;
    }

        = null!;

    [Required]
    [MaxLength(10)]
    public string Number
    {
        get;
        set;
    }

        = null!;

    [MaxLength(10)]
    public string Complement
    {
        get;
        set;
    }

        = null!;

    [MaxLength(50)]
    public string Neighborhood
    {
        get;
        set;
    }

        = null!;

    [Required]
    [MaxLength(9)]
    public string ZipCode
    {
        get;
        set;
    }

        = null!;

    [Required]
    public Guid StateId
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

        = null!;

    [ForeignKey(nameof(StateId))]
    public virtual StateModel State
    {
        get;
        set;
    }

        = null!;

    public virtual List<StockMovementModel> StockMovements
    {
        get;
        set;
    }

    = [];
}
