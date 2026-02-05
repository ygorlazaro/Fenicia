using Fenicia.Common.Enums;

namespace Fenicia.Externals.Email;

public interface IBrevoProvider
{
    void Send(EmailTemplate template, string email, string name, Dictionary<string, object>? parameters);
}
