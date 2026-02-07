namespace Fenicia.Common.Data.Responses.Auth;

public class UserResponse
{
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public Guid Id { get; set; }
}