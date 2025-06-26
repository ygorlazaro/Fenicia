namespace Fenicia.Common.Database;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public abstract class BaseModel
{
    [Key]
    public Guid Id { get; set; }

    [JsonIgnore]
    public DateTime Created { get; set; }

    [JsonIgnore]
    public DateTime? Updated { get; set; }

    [JsonIgnore]
    public DateTime? Deleted { get; set; }
}
