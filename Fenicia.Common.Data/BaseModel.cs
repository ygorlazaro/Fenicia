using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data;

public abstract class BaseModel
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    [JsonIgnore]
    public DateTime Created { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public DateTime? Updated { get; set; }

    [JsonIgnore]
    public DateTime? Deleted { get; set; }
}
