using Fenicia.Auth.Domains.User;
using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Token;

public interface ITokenService
{
    ApiResponse<string> GenerateToken(
        UserResponse user,
        string[] roles,
        Guid companyId,
        List<ModuleType> modules
    );
}
