namespace Fenicia.Web.Providers.Auth;

public class AuthManager
{
    private string? jwtToken;
    public bool IsAuthenticated => !string.IsNullOrEmpty(jwtToken);
    public string? JwtToken => jwtToken;

    public event Action? OnAuthStateChanged;

    public void SetToken(string token)
    {
        jwtToken = token;
        OnAuthStateChanged?.Invoke();
    }

    public void Logout()
    {
        jwtToken = null;
        OnAuthStateChanged?.Invoke();
    }
}
