namespace Fenicia.Auth.Requests;

/// <summary>
/// Request model for a single module order detail
/// </summary>
public class OrderDetailRequest
{
    /// <summary>
    /// The unique identifier of the module being ordered
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid ModuleId { get; set; }
}
