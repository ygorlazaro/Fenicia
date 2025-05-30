namespace Fenicia.Auth.Responses;

public class NewUserResponse
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Guid Id { get; set; }
}
