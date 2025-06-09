using Fenicia.Auth.Responses;
using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Services.Interfaces;

public interface ITokenService
{
    ApiResponse<string> GenerateToken(
        UserResponse user,
        string[] roles,
        Guid companyId,
        List<ModuleType> modules
    );
}
