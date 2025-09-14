namespace Fenicia.Auth.Domains.Token;

using System.ComponentModel.DataAnnotations;

using Common;
using Common.Enums;

using Common.Database.Responses;

public interface ITokenService
{
    ApiResponse<string> GenerateToken([Required] UserResponse user, [Required] string[] roles, [Required] Guid companyId, [Required] List<ModuleType> modules);
}
