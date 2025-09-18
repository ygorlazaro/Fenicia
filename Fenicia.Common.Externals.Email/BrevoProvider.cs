namespace Fenicia.Common.Externals.Email;

using brevo_csharp.Api;
using brevo_csharp.Model;

using Enums;

using Configuration = brevo_csharp.Client.Configuration;

public class BrevoProvider : IBrevoProvider
{
    public void Send(EmailTemplate template, string email, string name, Dictionary<string, object>? parameters)
    {
        var config = new Configuration
        {
            ApiKey =
                         {
                             ["api-key"] = Environment.GetEnvironmentVariable("BREVO_API_KEY")
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
