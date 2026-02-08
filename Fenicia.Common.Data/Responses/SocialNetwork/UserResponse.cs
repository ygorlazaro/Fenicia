using Fenicia.Common.Data.Models.SocialNetwork;

namespace Fenicia.Common.Data.Responses.SocialNetwork;

public class UserResponse(UserModel model)
{
    public Guid Id { get; set; } = model.Id;

    public string Name { get; set; } = model.Name;

    public string Username { get; set; } = model.Username;

    public string? ImageUrl { get; set; } = model.ImageUrl;
}