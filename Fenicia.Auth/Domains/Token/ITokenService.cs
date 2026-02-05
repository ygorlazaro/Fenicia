using System.ComponentModel.DataAnnotations;

using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Auth.Domains.Token;

public interface ITokenService
{
    string GenerateToken([Required] UserResponse user);
}
