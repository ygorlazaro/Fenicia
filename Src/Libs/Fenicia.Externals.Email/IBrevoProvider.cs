using Fenicia.Common.Enums.External;

namespace Fenicia.Externals.Email;

public interface IBrevoProvider
{
    void Send(EmailTemplate template, string email, string name, Dictionary<string, object>? parameters);
}