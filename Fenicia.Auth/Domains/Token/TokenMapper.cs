using Fenicia.Common.DTOs.Auth.Token;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Auth.Domains.Token;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class TokenMapper
{
    public partial UserResponse MapToUserResponse(GenerateTokenResponse user);

    public TokenResponse MapToTokenResponse(string accessToken, string refreshToken, GenerateTokenResponse user)
    {
        return new TokenResponse(accessToken, refreshToken, MapToUserResponse(user));
    }
}
