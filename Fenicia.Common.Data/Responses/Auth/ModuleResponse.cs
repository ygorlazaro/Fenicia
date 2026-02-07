using System.Text.Json.Serialization;

using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Data.Responses.Auth;

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

    [JsonPropertyName("price")]
    public decimal Price
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
}