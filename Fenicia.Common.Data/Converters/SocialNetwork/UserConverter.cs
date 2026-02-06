using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Requests.SocialNetwork;
using Fenicia.Common.Data.Responses.SocialNetwork;

namespace Fenicia.Common.Data.Converters.SocialNetwork;

public static class UserConverter
{
    public static UserModel Convert(UserRequest request)
    {
        return new UserModel
        {
            Name = request.Name,
            Email = request.Email,
            Username = request.Username
        };
    }

    public static UserResponse Convert(UserModel model)
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
