namespace Fenicia.Auth.Domains.Token.Responses;

public record TokenResponse(string AccessToken, string RefreshToken, UserResponse User);