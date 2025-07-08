namespace Fenicia.Module.Basic.Domains.Employee;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Address;

using Common.Database;

using Position;

[Table(name: "employees")]
public class EmployeeModel : BaseModel
{
    [Required]
    [MaxLength(length: 50)]
    public string Name { get; set; } = null!;

    [MaxLength(length: 14)]
    public string? Cpf { get; set; } = null!;

    public Guid AddressId
    {
        get; set;
    }

    [Required]
    public Guid PositionId
    {
        get; set;
    }

    [ForeignKey(name: "PositionId")]
    public virtual PositionModel Position { get; set; } = null!;

    [ForeignKey(name: "AddressId")]
    public virtual AddressModel Address { get; set; } = null!;
}
