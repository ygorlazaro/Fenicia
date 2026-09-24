using Fenicia.Common.DTOs.Auth.Register;
using Fenicia.Common.DTOs.Auth.User;
using Fenicia.Common.DTOs.Auth.UserRole;

namespace Fenicia.Auth.Domains.Register;

/// <summary>
/// The RegisterMapper class provides mapping functionality to convert UserResponse objects into RegisterResponse objects. This is particularly useful in scenarios where user information needs to be transformed into a format suitable for registration responses, encapsulating user details and associated company information.
/// </summary>
public class RegisterMapper
{
    /// <summary>
    /// Maps a UserResponse object to a RegisterResponse object. This method extracts relevant user information and constructs a RegisterResponse, which includes user details and associated company information. It is used to transform the data received from the user service into a format suitable for registration responses.
    /// </summary>
    /// <param name="user">The UserResponse object containing user details.</param>
    /// <returns>A RegisterResponse object containing the mapped user information.</returns>
    public static RegisterResponse MapToRegisterResponse(UserResponse user)
    {
        return new RegisterResponse(
            user.Id,
            user.Name,
            user.Email,
            new UserCompanyResponse(user.Id, string.Empty, user.Company.Id, user.Company.Name, user.Company.Cnpj));
    }
}
