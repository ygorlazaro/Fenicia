using System.Text.Json.Serialization;

using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Enums;

namespace Fenicia.Common.Database.Responses;

[Serializable]
public class ModuleResponse
{
    [JsonPropertyName("id")]
    public Guid Id
    {
        get; set;
    }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("amount")]
    public decimal Amount
    {
        get; set;
    }

    [JsonPropertyName("type")]
    public ModuleType Type
    {
        get; set;
    }

    [JsonPropertyName("submodules")]
    public SubmoduleResponse[] Submodules { get; set; } = [];

    public static ModuleResponse Convert(ModuleModel module)
    {
        return new ModuleResponse
        {
            Id = module.Id,
            Name = module.Name,
            Amount = module.Amount,
            Type = module.Type,
            Submodules = SubmoduleResponse.Convert(module.Submodules)
        };
    }

    public static List<ModuleResponse> Convert(List<ModuleModel> modules)
    {
        return [.. modules.Select(ModuleResponse.Convert)];
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ModuleResponse other)
        {
            return false;
        }

        return Id == other.Id &&
               Name == other.Name &&
               Amount == other.Amount &&
               Type == other.Type &&
               Submodules.SequenceEqual(other.Submodules);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Amount, Type, Submodules);
    }
}
