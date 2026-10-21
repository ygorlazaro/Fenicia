using Fenicia.Common.DTOs.Auth.Register;
using Fenicia.Common.DTOs.Auth.User;

namespace Fenicia.Auth.Domains.Register;

/// <summary>
/// Mapper class for mapping between RegisterRequest and RegisterResponse.
/// </summary>
public static class RegisterMapper
{
    /// <summary>
    /// Maps a RegisterRequest to a RegisterResponse.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <param name="user">The created user model.</param>
    /// <param name="company">The created company model.</param>
    /// <returns>The registration response.</returns>
    public static RegisterResponse MapToRegisterResponse(UserResponse user)
    {
        return new RegisterResponse(
            user.Id,
            user.Name,
            user.Email,
            new UserCompanyResponse(user.Id, string.Empty, user.Id, string.Empty, string.Empty));
    }
}
