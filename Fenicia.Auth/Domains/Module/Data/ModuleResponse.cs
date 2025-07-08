namespace Fenicia.Auth.Domains.Module.Data;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using Common.Enums;

[Serializable]
public class ModuleResponse
{
    [Required]
    [JsonPropertyName(name: "id")]
    public Guid Id
    {
        get; set;
    }

    [Required]
    [StringLength(maximumLength: 30)]
    [JsonPropertyName(name: "name")]
    public string Name { get; set; } = null!;

    [Required]
    [Range(minimum: 0, maximum: 999999.99)]
    [JsonPropertyName(name: "amount")]
    public decimal Amount
    {
        get; set;
    }

    [Required]
    [JsonPropertyName(name: "type")]
    public ModuleType Type
    {
        get; set;
    }

    public static ModuleResponse Convert(ModuleModel module)
    {
        return new ModuleResponse
        {
            Id = module.Id,
            Name = module.Name,
            Amount = module.Amount,
            Type = module.Type
        };
    }

    public static List<ModuleResponse> Convert(List<ModuleModel> modules)
    {
        return modules.Select(ModuleResponse.Convert).ToList();
    }
}
