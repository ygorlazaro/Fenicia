using Fenicia.Auth.Domains.User.Data;
using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.Token.Logic;

public interface ITokenService
{
    ApiResponse<string> GenerateToken(
        UserResponse user,
        string[] roles,
        Guid companyId,
        List<ModuleType> modules
    );
}
