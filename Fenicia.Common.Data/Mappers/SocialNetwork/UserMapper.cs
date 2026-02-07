using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Requests.SocialNetwork;
using Fenicia.Common.Data.Responses.SocialNetwork;

namespace Fenicia.Common.Data.Mappers.SocialNetwork;

public static class UserMapper
{
    public static UserModel Map(UserRequest request)
    {
        return new UserModel
        {
            Name = request.Name,
            Email = request.Email,
            Username = request.Username
        };
    }

    public static UserResponse Map(UserModel model)
    {
        return new UserResponse
        {
            Id = model.Id,
            Name = model.Name,
            Username = model.Username,
            ImageUrl = model.ImageUrl
        };
    }
}