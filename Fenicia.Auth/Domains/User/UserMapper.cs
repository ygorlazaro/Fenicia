using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.User;
using Fenicia.Common.DTOs.Auth.UserRole;

namespace Fenicia.Auth.Domains.User;

/// <summary>
/// Mapper class for mapping between UserModel and UserResponse/UserRequest.
/// </summary>
public static class UserMapper
{
    /// <summary>
    /// Maps a UserModel to a UserResponse.
    /// </summary>
    /// <param name="user">The user model.</param>
    /// <returns>The user response.</returns>
    public static UserResponse MapToUserResponse(UserModel user)
    {
        return new UserResponse(
            user.Id,
            user.Name,
            user.Email,
            new CompanyResponse(Guid.Empty, string.Empty, string.Empty));
    }

    /// <summary>
    /// Maps a UserRequest to a UserModel.
    /// </summary>
    /// <param name="request">The user request.</param>
    /// <returns>The user model.</returns>
    public static UserModel MapToUserModel(UserRequest request)
    {
        return new UserModel
        {
            Id = request.Id,
            Email = request.Email,
            Name = request.Name
        };
    }
}