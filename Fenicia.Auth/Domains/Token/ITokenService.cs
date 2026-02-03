using System.ComponentModel.DataAnnotations;

using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.Token;

public interface ITokenService
{
    string GenerateToken([Required] UserResponse user);
}
