using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Database;

namespace Fenicia.Module.Basic.Contexts.Models;

[Table("employees")]
public class EmployeeModel: BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;
    
    [MaxLength(14)]
    public string? Cpf { get; set; } = null!;
    
    public Guid AddressId { get; set; }
    
    [Required]
    public Guid PositionId { get; set; }
    
    [ForeignKey("PositionId")]
    public virtual PositionModel Position { get; set; } = null!;
    
    [ForeignKey("AddressId")]
    public virtual AddressModel Address { get; set; } = null!;
}