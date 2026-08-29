using Fenicia.Auth.Domains.Register.DTOs;
using Fenicia.Auth.Domains.User.DTOs;

namespace Fenicia.Auth.Domains.Register;

public static partial class RegisterMapper
{
    public static RegisterResponse MapToRegisterResponse(this CreateNewUserResponse userResponse)
    {
        return new RegisterResponse(
            userResponse.Id,
            userResponse.Name,
            userResponse.Email,
            userResponse.Company);
    }
}
