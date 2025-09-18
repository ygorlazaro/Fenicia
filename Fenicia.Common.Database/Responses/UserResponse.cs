namespace Fenicia.Common.Database.Responses;

using Models.Auth;

public class UserResponse
{
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public Guid Id
    {
        get; set;
    }

    public static UserResponse Convert(UserModel user)
    {
        return new UserResponse { Id = user.Id, Name = user.Name, Email = user.Email };
    }
}
