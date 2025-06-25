using Fenicia.Common.Enums;

namespace Fenicia.Common.Externals.Email;

public interface IBrevoProvider
{
    void Send(EmailTemplate template, string email, string name, Dictionary<string, object>? parameters);
}
