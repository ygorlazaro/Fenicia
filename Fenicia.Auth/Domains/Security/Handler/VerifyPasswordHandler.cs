using Fenicia.Auth.Domains.Security.Query;

using MediatR;

namespace Fenicia.Auth.Domains.Security.Services;

public class VerifyPasswordService : IRequestHandler<VerifyPasswordQuery, bool>
{
    public virtual bool Handle(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public Task<bool> Handle(VerifyPasswordQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Handle(request.Password, request.HashedPassword));
    }
}
