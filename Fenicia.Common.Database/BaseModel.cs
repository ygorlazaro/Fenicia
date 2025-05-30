using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Database;

public abstract class BaseModel
{
    [Key]
    public Guid Id { get; set; }

    public DateTime Created { get; set; } = DateTime.Now;
    
    public DateTime? Updated { get; set; }
    
    public DateTime? Deleted { get; set; }
}