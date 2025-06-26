using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Module.Data;

/// <summary>
/// Represents a response model containing module information for API responses
/// </summary>
/// <remarks>
/// This class is used to transfer module data between the service layer and API controllers
/// </remarks>
[Serializable]
public class ModuleResponse
{
    /// <summary>
    /// Gets or sets the unique identifier for the module
    /// </summary>
    /// <value>A globally unique identifier (GUID) that uniquely identifies this module</value>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    [Required]
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the module
    /// </summary>
    /// <value>The human-readable name of the module</value>
    /// <example>Analytics Pro</example>
    [Required]
    [StringLength(30)]
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the price of the module
    /// </summary>
    /// <value>The decimal amount representing the module's price</value>
    /// <example>199.99</example>
    [Required]
    [Range(0, 999999.99)]
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the type classification of the module
    /// </summary>
    /// <value>An enumeration value representing the module's type category</value>
    [Required]
    [JsonPropertyName("type")]
    public ModuleType Type { get; set; }
}
