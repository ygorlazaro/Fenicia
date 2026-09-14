using Fenicia.Common.DTOs.Auth.Register;
using Fenicia.Common.DTOs.Auth.User;

namespace Fenicia.Auth.Domains.Register;

public static class RegisterMapper
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