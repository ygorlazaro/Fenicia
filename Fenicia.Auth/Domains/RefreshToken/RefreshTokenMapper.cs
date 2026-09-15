using Fenicia.Common.DTOs.Auth.RefreshToken;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Auth.Domains.RefreshToken;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RefreshTokenMapper
{
    public partial GenerateRefreshTokenResponse MapToGenerateRefreshTokenResponse(RefreshTokenModel token);

    public partial ValidateTokenResponse MapToValidateTokenResponse(RefreshTokenModel token);

    public partial InvalidateRefreshTokenResponse MapToInvalidateRefreshTokenResponse(RefreshTokenModel token);
}
