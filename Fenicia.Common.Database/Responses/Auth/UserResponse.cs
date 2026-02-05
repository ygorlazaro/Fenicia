using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Common.Database.Responses.Auth;

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

    public override bool Equals(object? obj)
    {
        if (obj is not UserResponse other)
        {
            return false;
        }

        return Id == other.Id &&
               Name == other.Name &&
               Email == other.Email;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Email);
    }
}
