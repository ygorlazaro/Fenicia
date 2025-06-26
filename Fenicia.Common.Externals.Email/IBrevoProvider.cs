namespace Fenicia.Common.Externals.Email;

using Enums;

public interface IBrevoProvider
{
    void Send(EmailTemplate template, string email, string name, Dictionary<string, object>? parameters);
}
