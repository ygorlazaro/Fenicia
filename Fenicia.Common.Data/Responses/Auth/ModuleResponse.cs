using System.Text.Json.Serialization;

using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Common.Data.Responses.Auth;

[Serializable]
public class ModuleResponse()
{
    public ModuleResponse(ModuleModel model): this()
    {
        this.Id = model.Id;
        this.Name = model.Name;
        this.Price = model.Price;
        this.Type = model.Type;
        this.Submodules = [..model.Submodules.Select(submodule => new SubmoduleResponse(submodule)).ToList()];
    }

    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("type")]
    public ModuleType Type { get; set; }

    [JsonPropertyName("submodules")]
    public SubmoduleResponse[] Submodules { get; set; } = [];
}