using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fenicia.Common.Data;

public abstract class BaseModel
{
    [Key]
    public Guid Id
    {
        get; set;
    }

    protected BaseModel()
    {
        Id = Guid.NewGuid();
        Created = DateTime.UtcNow;
    }

    [JsonIgnore]
    public DateTime Created
    {
        get; set;
    }

    [JsonIgnore]
    public DateTime? Updated
    {
        get; set;
    }

    [JsonIgnore]
    public DateTime? Deleted
    {
        get; set;
    }
}
