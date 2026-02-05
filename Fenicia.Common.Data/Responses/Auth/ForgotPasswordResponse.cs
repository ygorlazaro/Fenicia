using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Responses.Auth;

[Serializable]
public class ForgotPasswordResponse
{
    public Guid Id
    {
        get; set;
    }

    public static ForgotPasswordResponse Convert(ForgotPasswordModel model)
    {
        return new ForgotPasswordResponse
        {
            Id = model.Id
        };
    }
}
