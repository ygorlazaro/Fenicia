namespace Fenicia.Auth.Domains.Security.HashPassword;

public class HashPasswordHandler
{
    public virtual string Handle(string password)
    {
        return password.Hash();
    }
}
