using brevo_csharp.Api;
using brevo_csharp.Model;

using Fenicia.Common.Enums;

using Configuration = brevo_csharp.Client.Configuration;

namespace Fenicia.Common.Externals.Email;

public class BrevoProvider : IBrevoProvider
{
    public void Send(EmailTemplate template, string email, string name, Dictionary<string, object>? parameters)
    {
        var config = new Configuration
        {
            ApiKey =
            {
                ["api-key"] = "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"
            }
        };

        var client = new TransactionalEmailsApi(config);
        var sendSmtpEmail = new SendSmtpEmail
        {
            To = [new SendSmtpEmailTo(email, name)],
            TemplateId = (int)template,
            Params = parameters
        };

        client.SendTransacEmail(sendSmtpEmail);
    }
}
