using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Responses.Auth;

[Serializable]
public class ForgotPasswordResponse()
{
    public ForgotPasswordResponse(ForgotPasswordModel model): this()
    {
        this.Id = model.Id;
    }

    public Guid Id { get; set; } = Guid.Empty;
}