using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Database;

public abstract class BaseModel
{
    [Key]
    public Guid Id { get; set; }

    [JsonIgnore]
    public DateTime Created { get; set; } = DateTime.Now;
    
    [JsonIgnore]
    public DateTime? Updated { get; set; }
    
    [JsonIgnore]
    public DateTime? Deleted { get; set; }
}