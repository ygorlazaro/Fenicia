namespace Fenicia.Common.DTOs.Auth.Token;

public class GenerateTokenStringQuery()
{
    public GenerateTokenStringQuery(GenerateTokenResponse user)
        : this()
    {
        User = user;
    }

    public GenerateTokenResponse User { get; set; } = new();
}
