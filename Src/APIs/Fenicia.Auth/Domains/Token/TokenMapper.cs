using Fenicia.Auth.Domains.Token.DTOs;

namespace Fenicia.Auth.Domains.Token;

public static class TokenMapper
{
    public static UserResponse MapToUserResponse(this GenerateTokenResponse user)
    {
        return new UserResponse(user.Id, user.Name, user.Email);
    }

    public static TokenResponse MapToTokenResponse(this string token, string refreshToken, GenerateTokenResponse user)
    {
        return new TokenResponse(token, refreshToken, user.MapToUserResponse());
    }
}