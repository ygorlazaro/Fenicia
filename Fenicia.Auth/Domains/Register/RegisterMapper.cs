using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Register;

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
    public static RegisterResponse MapToRegisterResponse(RegisterRequest request, UserModel user, CompanyModel company)
    {
        return new RegisterResponse(
            user.Id,
            user.Name,
            user.Email,
            new UserCompanyResponse(
                user.Id,
                company.Id,
                company.Name,
                company.Cnpj));
    }
}
