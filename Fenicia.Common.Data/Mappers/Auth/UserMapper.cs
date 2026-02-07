using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Common.Data.Mappers.Auth;

public static class UserMapper
{
    public static UserResponse Map(UserModel user)
    {
        return new UserResponse { Id = user.Id, Name = user.Name, Email = user.Email };
    }
}