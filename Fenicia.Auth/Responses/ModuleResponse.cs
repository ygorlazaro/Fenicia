using Fenicia.Common.Enums;

namespace Fenicia.Auth.Responses;

/// <summary>
/// Response model containing module information
/// </summary>
public class ModuleResponse
{
    /// <summary>
    /// The unique identifier of the module
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the module
    /// </summary>
    /// <example>Analytics Pro</example>
    public string Name { get; set; } = null!;

    /// <summary>
    /// The price of the module
    /// </summary>
    /// <example>199.99</example>
    public decimal Amount { get; set; }

    /// <summary>
    /// The type of the module
    /// </summary>
    public ModuleType Type { get; set; }
}
