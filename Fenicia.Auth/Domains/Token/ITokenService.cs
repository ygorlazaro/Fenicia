using System.ComponentModel.DataAnnotations;

using Fenicia.Common;

using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.Token;

public interface ITokenService
{
    ApiResponse<string> GenerateToken([Required] UserResponse user);
}
