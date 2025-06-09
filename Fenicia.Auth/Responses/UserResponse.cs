namespace Fenicia.Auth.Responses;

/// <summary>
/// Response model containing user information
/// </summary>
public class UserResponse
{
    /// <summary>
    /// The full name of the user
    /// </summary>
    /// <example>John Doe</example>
    public string Name { get; set; } = null!;

    /// <summary>
    /// The email address of the user
    /// </summary>
    /// <example>user@example.com</example>
    public string Email { get; set; } = null!;

    /// <summary>
    /// The unique identifier of the user
    /// </summary>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid Id { get; set; }
}
